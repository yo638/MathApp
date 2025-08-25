using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using MathApp.Models.DbModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.IO;
namespace MathApp.Services.Interfaces
{
    public interface IFileService
    {
        (MemoryStream stream, string contentType, string filename) GenerateTopicFile(Topic topic);
        (MemoryStream stream, string contentType, string filename) CreateAndEncryptTopicFile(Topic topic, string password);
        (MemoryStream stream, string contentType, string filename) CreateAndEncryptTopicFileWithAnswers(Topic topic, string password);
        (MemoryStream stream, string contentType, string filename) GenerateTopicFileWithAnswers(Topic topic);
    }
}
