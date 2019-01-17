﻿using System;
using System.Collections.Generic;
using Ascon.Pilot.Web.Models;

namespace Ascon.Pilot.Web.ViewModels
{
    /// <summary>
    /// Модель представления пользовательских папок
    /// </summary>
    public class UserPositionViewModel
    {
        /// <summary>
        /// Id current object id
        /// </summary>
        public Guid CurrentFolderId { get; set; }

        public bool IsFile { get; set; }
        public long Version { get; set; }

        /// <summary>
        /// Тип панели управления файлами
        /// </summary>
        public FilesPanelType FilesPanelType { get; set; }
        
        /// <summary>
        /// Модель боковой панели
        /// </summary>
        public SidePanelViewModel SidePanel { get; set; }

        /// <summary>
        /// Список файлов
        /// </summary>
        public List<FileViewModel> Files { get; set; }

        /// <summary>
        /// Навигационная цепочка типа "Хлебные крошки", представляющая
        /// путь по файлово системе от корня до рабочего каталога, который в данных момент
        /// просматоривает пользователь.
        /// </summary>
        public Queue<KeyValuePair<Guid, string>> BreadCrumbs { get; set; }
    }
}