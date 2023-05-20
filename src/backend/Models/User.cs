﻿using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
using System.ComponentModel.DataAnnotations;

namespace backend.Models
{
    public class User
    {
        //sets primary key
        [BsonId]
        public string? UserId { get; set; }
        public string? Username { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? Email { get; set; }
        public string? Password { get; set; }
    }

    public class RegistrationRequest
    {
        [Required]
        public string? Username { get; set; }
        [Required]
        public string? FirstName { get; set; }
        [Required]
        public string? LastName { get; set; }
        [Required,EmailAddress]
        public string? Email { get; set; }
        [Required]
        public string? Password { get; set; }
    }
}