﻿using ReportPortal.DAL.Enums;

namespace Models.Dto
{
    public class UserForCreationDto
    {
        public string Email { get; set; }
        public string Password { get; set; }
        public UserRole Role { get; set; } 
    }
}
