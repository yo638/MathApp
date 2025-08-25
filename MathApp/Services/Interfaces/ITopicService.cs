using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using Microsoft.AspNetCore.Mvc;
using MathApp.Models.BusinessModels;
using MathApp.Models.DbModels;
using System.Collections.Generic;
using System.IO;
using System;

namespace MathApp.Services.Interfaces
{
    public interface ITopicService
    {
        bool CreateTopic(Topic topic);
        IEnumerable<Topic> GetTopicsByUser(int idUser, string topicType);
        IEnumerable<Topic> SearchTopics(int idUser, SearchCriteriaTopic criteria);
        Topic GetTopicByID(int idTopic);
        bool UpdateTopic(Topic newTopic);
        bool AddTopicToRecycleBin(int idTopic);
        bool AddTopicAndMathProblemsToRecycleBin(int idTopic);
        IEnumerable<Topic> GetRecycledTopicsByUser(int idUser);
        bool RecoverTopicFromRecycleBin(int idTopic);
        bool RecoverTopicAndMathProblemsFromRecycleBin(int idTopic);
        bool DeleteTopic(int idTopic);
        bool DeleteTopicAndMathProblems(int idTopic);
    }
}
