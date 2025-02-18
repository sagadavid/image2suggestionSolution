﻿using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace image2suggestion.Models
{
    public class Photo
    {
        [Key]
        public int Id { get; set; }

        //[Required(ErrorMessage = "title your photo")]
        [Column(TypeName = "nvarchar(50)")]
        public string Title { get; set; }

        [Column(TypeName = "nvarchar(50)")]
        public string PhotoName { get; set; }

        [NotMapped]
        [DisplayName("Upload Photo")]
        public IFormFile PhotoInIForm { get; set; }

        //?[DisplayName("Photo belongs to")]
        public int SuggestionID { get; set; }
        public Suggestion Suggestion { get; set; }

        //[NotMapped]
        //public string PhotoPath { get; set; }

        //[NotMapped]
        //[DisplayName("Size in MB")]
        //public decimal DisplaySize { get; set; }

        //[NotMapped]
        //public byte[] PhotoInBytes { get; set; }

        
        //-The property 'Photo.PhotoInIForm' is of an interface type ('IFormFile').
        //If it is a navigation, manually configure the relationship for
        //this property by casting it to a mapped entity type. Otherwise,
        //ignore the property using the [NotMapped] attribute
        //or 'Ignore' in 'OnModelCreating'.


    }
}
