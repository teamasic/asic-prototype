﻿using AttendanceSystemIPCamera.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace AttendanceSystemIPCamera.Framework.ViewModels
{
    public class RecordViewModel : BaseViewModel<Record>
    {
        public int Id { get; set; }
        public AttendeeGroupViewModel AttendeeGroup {get; set;}
        public AttendeeViewModel Attendee => AttendeeGroup?.Attendee;
        public SessionViewModel Session { get; set; }
        public bool Present { get; set; }
        public string Image { get; set; }
    }
    public class RecordSearchViewModel
    {
        [Required]
        public string AttendeeCode { get; set; }
        [Required]
        public DateTime StartTime { get; set; }
        [Required]
        public DateTime EndTime { get; set; }
    }

    public class RecordNetworkViewModel : BaseViewModel<Record>
    {
        public int Id { get; set; }
        public string AttendeeCode { get; set; }
        public int AttendeeGroupId { get; set; }
        public bool Present { get; set; }
        public int SessionId { get; set; }
        public DateTime? UpdateTime { get; set; }
        public ChangeRequestSimpleViewModel ChangeRequest { get; set; }
    }
    public class RecordInSyncData : BaseViewModel<Record>
    {
        public int Id { get; set; }
        public SessionInSyncData Session { get; set; }
        public AttendeeGroupInSyncViewModel AttendeeGroup { get; set; }
        public bool Present { get; set; }
        public DateTime? UpdateTime { get; set; }
    }
}

