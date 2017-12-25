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
            var personId = 1; //TODO ���������� ������ ID �������� ������������

            //����� ������
            var newObject = new DObject()
            {
                Id = Guid.NewGuid(),
                ParentId = parentId,
                TypeId = typeId,
                CreatorId = personId, // TODO Id �������� ������������
                Created = DateTime.UtcNow,
            };

            //���������� ��������� � ������
            newObject.Attributes.Add("Name", "��� �������");
            newObject.Attributes.Add("AttributeName", 1);

            //���������� ��������� md5 �����, ������� ����� ��������� �� ������ c ��� filename
            string md5 = "";
            //using (var md5Hasher = new MD5CryptoServiceProvider())
            //    md5 = stream.CopyToAndHash(localFileStorageStream, md5Hasher);

            //���������� ��������� ������ �����
            long size = 0;

            //���������� ������ � ������
            var body = new DFileBody
            {
                Accessed = DateTime.UtcNow,
                Created = DateTime.UtcNow,
                Modified = DateTime.UtcNow,
                Id = Guid.NewGuid(),
                Md5 = md5, // TODO ���������� ��������� md5 �����, ������� ����� ��������� �� ������
                Size = size // TODO ������� ������ �����
            };

            //�������� ������ ����
            var file = new DFile
            {
                Body = body,
                Name = "File" // TODO ��� �����
            };

            //���� � ����� ���� �������� ������� ������� ��
            //��� ������ ��������, � �� ���� �������. ���� ������� ������ ���� �������� � XPS, ���� ����������� XPS ��������
            //file.Signatures.Add(new DSignature() { });

            newObject.ActualFileSnapshot.AddFile(file, personId);
            
            //������� ����� ������ �������� � ����
            var parentChange = EditObject(parentId);
            parentChange.New.Children.Add(new DChild() { ObjectId = newObject.Id, TypeId = typeId });

            ////�� �������� ��������� ����� �� ������
            //foreach (var access in parentChange.New.Access)
            //{
            //    newObject.Access.Add(access.Key, new Access(access.Value.AccessLevel, access.Value.ValidThrough, access.Value.IsInheritable, access.Value.IsInherited));
            //}
            ////�������� ����� ��������� �� ������
            //newObject.Access.Add(personId, new Access(AccessLevel.ViewEdit, DateTime.MaxValue, true, false));
            
            //�������� ��������� ��� ������ �������
            var change = new DChange() { New = newObject, Old = null }; // Old = null - �������� �������. New = null - ������������� �������� �������

            //��� ����, ����� ������ ������ ��������� ���������� ������� ������ ���� DChangesetData
            var changesetData = new DChangesetData();
            changesetData.Identity = Guid.NewGuid();

            //������� ��������� (��������) �������� �������
            changesetData.Changes.Add(change);
            //������� ��������� ������������� ������� (���������� �����)
            changesetData.Changes.Add(parentChange);

            //������� �������������� ������, ������� ���� ��������� �� ������
            changesetData.NewFileBodies.Add(file.Body.Id);

            //����� ��� ��� ��������� ��������� ���������, ���������� ��������� �� ������ ���� �����
            var uploader = new ChangesetUploader(filename, _fileArchiveApi, changesetData);
            uploader.Upload();

            //�������� ��������� �� ������
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
                //�������� ������ � ��������� ����� �����
                pos = _fileArchiveApi.GetFilePosition(string.Empty, file.Body.Id);
                if (pos > file.Body.Size)
                    throw new Exception(string.Format("File with id {0} is corrupted", file.Body.Id));
            }

            // ����� � ��������� ��� ����������� ������
            _uploaded += pos;

            //�������� ���� �� ������
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
                        //Logger.Error("��� �������� ����� ��������� ������", e);
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

            var chunkSize = 512 * 1024; //512 kb ����� ��������
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