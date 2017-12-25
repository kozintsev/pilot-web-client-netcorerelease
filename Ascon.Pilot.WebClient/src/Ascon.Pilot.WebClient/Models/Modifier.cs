using System;
using System.Linq;
using Ascon.Pilot.Core;
using Ascon.Pilot.Server.Api.Contracts;
using System.IO;

namespace Ascon.Pilot.WebClient.Models
{
    public class Modifier
    {
        private readonly IServerApi _serverApi;
        private readonly IFileArchiveApi _fileArchiveApi;

        public Modifier(IServerApi serverApi, IFileArchiveApi fileArchiveApi)
        {
            _serverApi = serverApi;
            _fileArchiveApi = fileArchiveApi;
        }

        public void CreateObject(Guid id, Guid parentId, int typeId, string filename)
        {
            var personId = 1; //TODO необъодимо задать ID текущего пользователя

            //Новый объект
            var newObject = new DObject()
            {
                Id = Guid.NewGuid(),
                ParentId = parentId,
                TypeId = typeId,
                CreatorId = personId, // TODO Id текущего пользователя
                Created = DateTime.UtcNow,
            };

            //добавление атрибутов в объект
            newObject.Attributes.Add("Name", "Имя объекта");
            newObject.Attributes.Add("AttributeName", 1);

            //необходимо сосчитать md5 файла, который будет отправлен на сервер c исп filename
            string md5 = "";
            //using (var md5Hasher = new MD5CryptoServiceProvider())
            //    md5 = stream.CopyToAndHash(localFileStorageStream, md5Hasher);

            //необходимо сосчитать размер файла
            long size = 0;

            //Добавление файлов в объект
            var body = new DFileBody
            {
                Accessed = DateTime.UtcNow,
                Created = DateTime.UtcNow,
                Modified = DateTime.UtcNow,
                Id = Guid.NewGuid(),
                Md5 = md5, // TODO необходимо сосчитать md5 файла, который будет отправлен на сервер
                Size = size // TODO считаем размер файла
            };

            //Создадим объект фала
            var file = new DFile
            {
                Body = body,
                Name = "File" // TODO имя файла
            };

            //Если у фйала есть цифровые подписи добавим их
            //Это только описание, а не сами подписи. Сами подписи должны быть встроены в XPS, если добавляется XPS документ
            //file.Signatures.Add(new DSignature() { });

            newObject.ActualFileSnapshot.AddFile(file, personId);
            
            //Добавим новый объект родителю в дети
            var parentChange = EditObject(parentId);
            parentChange.New.Children.Add(new DChild() { ObjectId = newObject.Id, TypeId = typeId });

            ////НЕ ЗАБЫВАЕМ ПРОПИСАТЬ ПРАВА НА ОБЪЕТК
            //foreach (var access in parentChange.New.Access)
            //{
            //    newObject.Access.Add(access.Key, new Access(access.Value.AccessLevel, access.Value.ValidThrough, access.Value.IsInheritable, access.Value.IsInherited));
            //}
            ////Добавить права создателя на объект
            //newObject.Access.Add(personId, new Access(AccessLevel.ViewEdit, DateTime.MaxValue, true, false));
            
            //Создадим изменение для нового объекта
            var change = new DChange() { New = newObject, Old = null }; // Old = null - создание объекта. New = null - безвозвратное удаление объекта

            //Для того, чтобы сервер принял изменения необходимо создать объект типа DChangesetData
            var changesetData = new DChangesetData();
            changesetData.Identity = Guid.NewGuid();

            //Добавим изменение (создание) текущего объекта
            changesetData.Changes.Add(change);
            //Добавим изменение родительского объекта (добавление детей)
            changesetData.Changes.Add(parentChange);

            //Добавим идентификаторы файлов, которые были загружены на сервер
            changesetData.NewFileBodies.Add(file.Body.Id);

            //Перед тем как отправить созданные изменения, необходимо отправить на сервер тело файла
            var uploader = new ChangesetUploader(filename, _fileArchiveApi, changesetData);
            uploader.Upload();

            //отправим изменение на сервер
            _serverApi.Change(changesetData);
        }

        public DChange EditObject(Guid objectId)
        {
            var objects = _serverApi.GetObjects(new[] { objectId});
            var old = objects.First();
            
            var changed = old.Clone();
            var change = new DChange { Old = old, New = changed };
            return change;
        }
    }

    public class ChangesetUploader
    {
        private const long MIN_RESUME_UPLOAD_FILE_SIZE = 50 * 1024 * 1024;
        private readonly IFileArchiveApi _fileArchiveApi;
        private readonly DChangesetData _changeset;
        private long _uploaded;
        private readonly string _filename;

        public ChangesetUploader(string filename, IFileArchiveApi fileArchiveApi, DChangesetData changeset)
        {
            if (!File.Exists(filename))
                throw new ArgumentException("filename");
            if (fileArchiveApi == null)
                throw new ArgumentNullException("fileArchiveApi");
            if (changeset == null)
                throw new ArgumentNullException("changeset");
            
            _fileArchiveApi = fileArchiveApi;
            _changeset = changeset;
            _filename = filename;
        }

        private DFile FindFileBody(Guid id)
        {
            foreach (var change in _changeset.Changes)
            {
                var file = change.New.ActualFileSnapshot.Files
                    .Union(change.New.PreviousFileSnapshots.SelectMany(x => x.Files))
                    .FirstOrDefault(x => x.Body.Id == id);

                if (file != null)
                    return file;
            }
            throw new Exception(string.Format("Not found file body for id {0}", id));
        }

        public void Upload()
        {
            foreach (var id in _changeset.NewFileBodies)
            {
                var body = FindFileBody(id);
                CreateFile(body);
            }
        }

        private void CreateFile(DFile file)
        {
            long pos = 0;
            if (file.Body.Size > MIN_RESUME_UPLOAD_FILE_SIZE)
            {
                //опросить сервер о состоянии этого файла
                pos = _fileArchiveApi.GetFilePosition(string.Empty, file.Body.Id);
                if (pos > file.Body.Size)
                    throw new Exception(string.Format("File with id {0} is corrupted", file.Body.Id));
            }

            // учтем в прогрессе уже загруженные данные
            _uploaded += pos;

            //отправим тело на сервер
            using (var fs = File.Open(_filename, FileMode.Open))
            {
                if (file.Body.Size != fs.Length)
                    throw new Exception(string.Format("Local file size is incorrect: {0}", file.Body.Id));

                var fileBody = file.Body;

                const int MAX_ATTEMPT_COUNT = 5;
                int attemptCount = 0;
                bool succeed = false;
                do
                {
                    UploadData(fs, file.Body.Id, pos);
                    try
                    {
                        _fileArchiveApi.PutFileInArchive(string.Empty, fileBody);
                        succeed = true;
                    }
                    catch (Exception e)
                    {
                        //Logger.Error("при загрузке файла произошла ошибка", e);
                        pos = 0;
                        _uploaded = 0;
                    }
                    attemptCount++;
                } while (!succeed && attemptCount < MAX_ATTEMPT_COUNT);

                if (!succeed)
                    throw new Exception(string.Format("Unable to upload file {0}", file.Body.Id));
            }
        }

        private void UploadData(Stream fs, Guid id, long pos)
        {
            if (fs.Length == 0)
            {
                _fileArchiveApi.PutFileChunk(string.Empty, id, new byte[0], 0);
                //_progressListener.Progress(_uploaded);
                return;
            }

            var chunkSize = 512 * 1024; //512 kb Можно изменить
            var buffer = new byte[chunkSize];

            fs.Seek(pos, SeekOrigin.Begin);
            while (pos < fs.Length)
            {
                var readBytes = fs.Read(buffer, 0, chunkSize);
                _fileArchiveApi.PutFileChunk(string.Empty, id, TrimBuffer(buffer, readBytes), pos);

                pos += readBytes;
                _uploaded += readBytes;

                //_progressListener.Progress(_uploaded);
            }
        }

        // the last chunk will almost certainly not fill the buffer, so it must be trimmed before returning
        private byte[] TrimBuffer(byte[] buffer, int size)
        {
            if (size < buffer.Length)
            {
                var trimmed = new byte[size];
                Array.Copy(buffer, trimmed, size);
                return trimmed;
            }
            return buffer;
        }
    }
}