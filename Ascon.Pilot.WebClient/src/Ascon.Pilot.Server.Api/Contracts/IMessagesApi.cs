using System;
using System.Collections;
using System.Collections.Generic;
using Ascon.Pilot.Core;

namespace Ascon.Pilot.Server.Api.Contracts
{
    public interface IMessagesApi
    {
        /// <summary>
        /// Открыть соединение
        /// </summary>
        void Open(int maxNotificationCount);
        /// <summary>
        /// Отправить сообщение
        /// </summary>
        /// <param name="message">сообщение</param>
        DateTime SendMessage(DMessage message);

        /// <summary>
        /// Возвращает первые N чатов, отсортированные по дате последнего сообщения
        /// </summary>
        /// <param name="personId">идентификатор пользователя</param>
        /// <param name="fromDateTimeServer">начало временного промежутка</param>
        /// <param name="toDateTimeServer">окончание временного промежутка</param>
        /// <param name="topN">количество чатов</param>
        /// <returns></returns>
        List<DChatInfo> GetChats(int personId, DateTime fromDateTimeServer, DateTime toDateTimeServer, int topN);

        /// <summary>
        /// Получить информацию о чате
        /// </summary>
        /// <param name="chatId">Идентификатор чат</param>
        /// <returns></returns>
        DChatInfo GetChat(Guid chatId);

        /// <summary>
        /// Получить сообщение о создании чата
        /// </summary>
        /// <param name="chatId"></param>
        /// <returns></returns>
        DMessage GetChatCreationMessage(Guid chatId);

        /// <summary>
        /// Получить список участников чата, измененный после <param name="fromDateUtc"></param>
        /// </summary>
        /// <param name="chatId">идентификтор чата</param>
        /// <param name="fromDateUtc">дата последнего обновления</param>
        /// <returns></returns>
        List<DChatMember> GetChatMembers(Guid chatId, DateTime fromDateUtc);

        /// <summary>
        /// Возвращает список сообщений в количестве не большем <param name="maxNumber"/>
        /// И общее число сообщений, удовлетворяющих условиям.
        /// Сообщения отсортированы в порядке убывания серверной даты.
        /// </summary>
        /// <param name="chatId">идентификатор чата</param>
        /// <param name="dateFromUtc">минимальная серверная дата сообщения</param>
        /// <param name="dateToUtc">максимальная серверная дата сообщений</param>
        /// <param name="maxNumber">максимальное количество возвращаемых сообщений</param>
        /// <returns></returns>
        Tuple<List<DMessage>, int> GetMessages(Guid chatId, DateTime dateFromUtc, DateTime dateToUtc, int maxNumber);
        
        /// <summary>
        /// Возвращает самое ранее непрочитанное сообщения в чате
        /// </summary>
        /// <param name="chatId">идентификатор чата</param>
        /// <returns></returns>
        DMessage GetLastUnreadMessage(Guid chatId);

        /// <summary>
        /// Пользователь печатает сообщение в чат
        /// </summary>
        /// <param name="chatId">идентификатор чата</param>
        void TypingMessage(Guid chatId);

        /// <summary>
        /// Возвращает сообщения, содержащие вложения
        /// </summary>
        /// <param name="chatId">идентификатор чата</param>
        /// <param name="fromServerDateUtc">минимальная серверная дата сообщения</param>
        /// <param name="toServerDateUtc">максимальная серверная дата сообщения</param>
        /// <param name="pageSize">максимальное количество возвращаемых сообщений</param>
        /// <returns></returns>
        List<DMessage> GetMessagesWithAttachments(Guid chatId, DateTime fromServerDateUtc, DateTime toServerDateUtc, int pageSize);

        ///// <summary>
        ///// Возвращает чаты связанные с объектом
        ///// </summary>
        ///// <param name="objectId">идентификатор объекта</param>
        ///// <param name="type">тип связи</param>
        ///// <returns></returns>
        List<DChatInfo> GetRelatedChats(int personId, Guid objectId, ChatRelationType type);
    }
}
