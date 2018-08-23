using System;
using Ascon.Pilot.Core;

namespace Ascon.Pilot.Server.Api.Contracts
{
    /// <summary>
    /// Интерфейс обратного вызова API сообщений
    /// </summary>
    public interface IMessageCallback
    {
        /// <summary>
        /// функция обратного вызова добавляения сообщения
        /// </summary>
        /// <param name="message"></param>
        void NotifyMessageCreated(NotifiableDMessage message);

        /// <summary>
        /// функция обратного вызова "пользователь Х печатает сообщение"
        /// </summary>
        /// <param name="chatId">id чата</param>
        /// <param name="personId">id пользователя, печатающего сообщение</param>
        void NotifyTypingMessage(Guid chatId, int personId);

        /// <summary>
        /// функция обратного вызова создания уведомления
        /// </summary>
        /// <param name="notification"></param>
        void CreateNotification(DNotification notification);

    }
}
