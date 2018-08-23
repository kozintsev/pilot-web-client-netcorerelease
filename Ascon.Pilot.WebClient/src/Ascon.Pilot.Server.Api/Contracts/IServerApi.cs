using System;
using System.Collections.Generic;
using Ascon.Pilot.Core;

namespace Ascon.Pilot.Server.Api.Contracts
{
    public interface IServerApi
    {
        /// <summary>
        /// открыть базу данных
        /// </summary>
        /// <returns>описание базы данных</returns>
        DDatabaseInfo OpenDatabase();

        /// <summary>
        /// Получить описание базы данных
        /// </summary>
        /// <param name="database">имя базы данных</param>
        /// <returns>описание базы данных</returns>
        DDatabaseInfo GetDatabase(string database);

        /// <summary>
        /// Получить текущие метаданные
        /// </summary>
        /// <param name="localVersion">версия метаданных</param>
        /// <returns>метаданные</returns>
        DMetadata GetMetadata(long localVersion);

        /// <summary>
        /// Получение значений настроек
        /// </summary>
        /// <param name="loadAll">Получить все настройки для всех пользователей. Требуются права администратора.</param>
        /// <returns>значения настроек</returns>
        DSettings GetSettings(bool loadAll);

        /// <summary>
        /// Изменение значений настроек. Для изменения значений обобщенных настроек требуются права администратора.
        /// </summary>
        /// <param name="change">изменение</param>
        void ChangeSettings(DSettingsChange change);

        /// <summary>
        /// Получить объекты
        /// </summary>
        /// <param name="ids">список идентификаторов</param>
        /// <returns>список объектов</returns>
        List<DObject> GetObjects(Guid[] ids);

        /// <summary>
        /// Получить изменения 
        /// </summary>
        /// <param name="first">с позиции</param>
        /// <param name="last">до позиции</param>
        /// <returns></returns>
        List<DChangeset> GetChangesets(long first, long last);

        /// <summary>
        /// Применить изменения объектов
        /// </summary>
        /// <param name="changes">изменения</param>
        /// <returns>слитые измения с др. клиентами</returns>
        DChangeset Change(DChangesetData changes);

        /// <summary>
        /// Получить часть тела файлыа
        /// </summary>
        /// <param name="id">идентификатор файла</param>
        /// <param name="pos">позиция</param>
        /// <param name="count">кол-во</param>
        /// <returns>часть тела файла</returns>
        byte[] GetFileChunk(Guid id, long pos, int count);

        /// <summary>
        /// Положить часть тела файла
        /// </summary>
        /// <param name="id">идентификатор файла</param>
        /// <param name="buffer">часть тела</param>
        /// <param name="pos">позиция</param>
        void PutFileChunk(Guid id, byte[] buffer, long pos);

        /// <summary>
        /// Получить текущую позицию при считывании тела файла
        /// </summary>
        /// <param name="id">идентификатор файла</param>
        /// <returns>позиция</returns>
        long GetFilePosition(Guid id);

        /// <summary>
        /// Загрузить всех пользователей базы данных
        /// </summary>
        /// <returns>список пользователей</returns>
        List<DPerson> LoadPeople();

        /// <summary>
        /// Получить список пользователей по идентификаторам
        /// </summary>
        /// <param name="ids">Идентификаторы</param>
        /// <returns>список пользователей</returns>
        List<DPerson> LoadPeopleByIds(int[] ids);

        /// <summary>
        /// Получить список организационных единиц
        /// </summary>
        /// <returns>список орг. единиц</returns>
        List<DOrganisationUnit> LoadOrganisationUnits();

        /// <summary>
        /// Получить список организационных единиц по идентификаторам
        /// </summary>
        /// <param name="ids">Идентификаторы</param>
        /// <returns>список орг. единиц</returns>
        List<DOrganisationUnit> LoadOrganisationUnitsByIds(int[] ids);

        /// <summary>
        /// Добавить условие поиска
        /// </summary>
        /// <param name="searchDefinition">описание</param>
        void AddSearch(DSearchDefinition searchDefinition);

        /// <summary>
        /// Удалить условие поиска
        /// </summary>
        /// <param name="searchDefinitionId">идентификатор описания поиска</param>
        void RemoveSearch(Guid searchDefinitionId);

        /// <summary>
        /// Запустить поиск по геометрии
        /// </summary>
        /// <param name="searchDefinition">условия поиска</param>
        void GeometrySearch(DGeometrySearchDefinition searchDefinition);

        /// <summary>
        /// Запустить поиск по файлам
        /// </summary>
        /// <param name="searchDefinition">условия поиска</param>
        void ContentSearch(DSearchDefinition searchDefinition);

        /// <summary>
        /// Обновить информацию о пользователе.
        /// </summary>
        /// <param name="updateInfo"></param>
        void UpdatePerson(DPersonUpdateInfo updateInfo);

        /// <summary>
        /// Обновить информацию о должности
        /// </summary>
        /// <param name="updateInfo"></param>
        void UpdateOrganisationUnit(DOrganisationUnitUpdateInfo updateInfo);
    }

    public interface IFileArchiveApi
    {
        /// <summary>
        /// Получить часть тела файла из архива
        /// </summary>
        /// <param name="id">идентификатор файла</param>
        /// <param name="pos">позиция</param>
        /// <param name="count">кол-во</param>
        /// <returns>часть тела файла</returns>
        byte[] GetFileChunk(Guid id, long pos, int count);

        /// <summary>
        /// Положить часть тела файла
        /// </summary>
        /// <param name="id">идентификатор файла</param>
        /// <param name="buffer">часть тела</param>
        /// <param name="pos">позиция</param>
        void PutFileChunk(Guid id, byte[] buffer, long pos);

        /// <summary>
        /// Получить текущую позицию при считывании тела файла
        /// </summary>
        /// <param name="id">идентификатор файла</param>
        /// <returns>позиция</returns>
        long GetFilePosition(Guid id);

        /// <summary>
        /// Зафиксировать отправленные данные в файловом архиве. 
        /// При возникновении ошибки (например, несовпадении контрольной суммы или размера файла) данные во временном хранилище очищаются.
        /// </summary>
        /// <param name="fileBody">Описание файла</param>
        void PutFileInArchive(DFileBody fileBody);
    }
}
