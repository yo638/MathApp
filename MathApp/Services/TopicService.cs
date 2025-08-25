using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using MathApp.Models.BusinessModels;
using MathApp.Models.DbModels;
using MathApp.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace MathApp.Services
{
    public class TopicService : ITopicService
    {
        private readonly math_appContext _context;

        public TopicService(math_appContext context)
        {
            _context = context;
        }
        public bool CreateTopic(Topic topic)
        {
            try
            {

                var itemsToRemove = topic.MathProblems.Where(x => x.Conditions == null).ToList();
                foreach (var item in itemsToRemove)
                {
                    topic.MathProblems.Remove(item);
                }                
                for (int i = 0; i < topic.MathProblems.Count; i++)
                {
                    topic.MathProblems.ElementAt(i).CreationDate = DateTime.Now;
                    topic.MathProblems.ElementAt(i).UpdateDate = DateTime.Now;
                }
                _context.Topics.Add(topic);
                _context.SaveChanges();

            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return false;
            }
            return true;
        }
        public IEnumerable<Topic> GetTopicsByUser(int idUser, string topicType)
        {
            List<Topic> topics = new List<Topic>();
            try
            {
                var IdRole = _context.Users.Where(x => x.Id == idUser).Select(x => x.IdRole).FirstOrDefault();

                if (IdRole == 1)
                {
                    topics = _context.Topics.Where(t => t.Deletionstatus == 1 && t.Type==topicType).Select(t => new Topic
                    {
                        Id = t.Id,
                        Name = t.Name,
                        Description = t.Description,
                        UpdateDate = t.UpdateDate,
                    }).OrderByDescending(t=>t.UpdateDate).ToList();
                }
                else
                {
                    topics = _context.Topics.Where(t => t.IdUser == idUser && t.Deletionstatus == 1 && t.Type == topicType).Select(t => new Topic
                    {
                        Id = t.Id,
                        Name = t.Name,
                        Description = t.Description,
                        UpdateDate = t.UpdateDate,
                    }).OrderByDescending(t => t.UpdateDate).ToList();
                }

                foreach (var topic in topics)
                {
                    topic.SetUpdatedTimeAgoProperty();
                }
                return topics;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return topics;
            }
        }
        public IEnumerable<Topic> SearchTopics(int idUser, SearchCriteriaTopic criteria)
        {
            List<Topic> topics = new List<Topic>();
            try
            {
                topics = _context.Topics.Include(t => t.IdCategories).Where(t => t.IdUser == idUser && t.Deletionstatus == 1).Select(t => new Topic
                {
                    Id = t.Id,
                    Name = t.Name,
                    Description = t.Description,
                    UpdateDate = t.UpdateDate,
                    CreationDate = t.CreationDate,
                }).ToList();

                //Searching in Name
                if (criteria.name != null) topics = topics.Where(t => t.Name.Contains(criteria.name, StringComparison.OrdinalIgnoreCase)).ToList();

                //Searching in Description
                if (criteria.description != null) topics = topics.Where(t => t.Description.Contains(criteria.description, StringComparison.OrdinalIgnoreCase)).ToList();

                //Searching in Either Name Or Description
                if (criteria.anywhere != null) topics = topics
                        .Where(t => t.Description.Contains(criteria.anywhere, StringComparison.OrdinalIgnoreCase)
                        || t.Name.Contains(criteria.anywhere, StringComparison.OrdinalIgnoreCase)).ToList();

                //Searching in Categories
                if (criteria.category.Grade != 0) topics = topics.Where(t => t.IdCategories.Any(c => c.Grade == criteria.category.Grade)).ToList();
                if (criteria.category.Difficulty != "X") topics = topics.Where(t => t.IdCategories.Any(c => c.Difficulty == criteria.category.Difficulty)).ToList();

                //Searching in CreationDate
                if (criteria.fromDate != null) topics = topics.Where(z => z.CreationDate >= DateTime.Parse(criteria.fromDate)).ToList();
                if (criteria.toDate != null) topics = topics.Where(z => z.CreationDate <= DateTime.Parse(criteria.toDate)).ToList();
                foreach (var topic in topics)
                {
                    topic.SetUpdatedTimeAgoProperty();
                }
                return topics;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return topics;
            }
        }
        public Topic GetTopicByID(int idTopic)
        {
            var topic = new Topic();
            try
            {

                topic = _context.Topics
                    .Include(t => t.IdCategories)
              .Include(t => t.MathProblems)
                  .ThenInclude(z => z.Answers)
              .Include(t => t.MathProblems)
                  .ThenInclude(z => z.Categories)
              .Where(t => t.Id == idTopic)
              .SingleOrDefault();
                var recycledMathProblems = topic.MathProblems.Where(x => x.Deletionstatus == 0).Select(x => x.Id).ToList();
                foreach (var id in recycledMathProblems)
                {
                    var itemToRemove = topic.MathProblems.FirstOrDefault(x => x.Id == id);
                    if (itemToRemove != null)
                    {
                        topic.MathProblems.Remove(itemToRemove);
                    }
                }
                topic.MathProblems = topic.MathProblems.OrderBy(x => x.Position).ToList();
                return topic;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return topic;
            }
        }
        public bool UpdateTopic(Topic newTopic)
        {
            try
            {
                var oldTopic = _context.Topics.Include(t => t.IdCategories)
                                            .Include(t => t.MathProblems)
                                                .ThenInclude(z => z.Answers)
                                            .Include(t => t.MathProblems)
                                                .ThenInclude(z => z.Categories)
                                            .Where(t => t.Id == newTopic.Id).SingleOrDefault();                
                if (oldTopic != null)
                {
                    // Update Categories
                    oldTopic.IdCategories.Clear();
                    foreach (var category in newTopic.IdCategories)
                    {
                        var newCategory = _context.Categories.Where(x=>x.Grade==category.Grade && x.Difficulty==category.Difficulty).FirstOrDefault();
                        oldTopic.IdCategories.Add(newCategory);
                    }

                    // Update MathProblems
                    var newMathProblemIds = newTopic.MathProblems.Where(z => z.Deletionstatus == 1).Select(z => z.Id).ToList();
                    var mathProblemssToRemove = oldTopic.MathProblems.Where(z => !newMathProblemIds.Contains(z.Id)).ToList();

                    foreach (var mathProblemToRemove in mathProblemssToRemove)
                    {
                        oldTopic.MathProblems.Where(x => x.Id == mathProblemToRemove.Id).FirstOrDefault().Deletionstatus = 0;
                    }
                    foreach (var newMathProblem in newTopic.MathProblems)
                    {
                        var oldMathProblem = oldTopic.MathProblems.FirstOrDefault(z => z.Id == newMathProblem.Id);

                        if (oldMathProblem != null)
                        {
                            oldMathProblem.UpdateDate = DateTime.Now;
                            oldMathProblem.Conditions = newMathProblem.Conditions;
                            oldMathProblem.Solution = newMathProblem.Solution;
                            oldMathProblem.Position = newMathProblem.Position;

                            // Update Answers
                            oldMathProblem.Answers.Clear();
                            foreach (var answer in newMathProblem.Answers)
                            {
                                var newAnswer = _context.Answers.Where(x => x.Name == answer.Name && x.Validity == answer.Validity && x.MathProblem == answer.MathProblem).FirstOrDefault();
                                if (newAnswer != null) { 
                                oldMathProblem.Answers.Add(newAnswer);
                                }
                                else oldMathProblem.Answers.Add(answer);
                            }

                            // Update MathProblem Categories
                            oldMathProblem.Categories.Clear();
                            foreach (var category in newMathProblem.Categories)
                            {
                                var newCategory = _context.Categories.Where(x => x.Grade == category.Grade && x.Difficulty == category.Difficulty).FirstOrDefault();
                                oldMathProblem.Categories.Add(newCategory);
                            }
                            _context.Entry(oldMathProblem).State = EntityState.Modified;
                        }
                        else
                        {
                            oldTopic.MathProblems.Add(newMathProblem);
                            _context.SaveChanges();
                        }

                    }

                    oldTopic.Name = newTopic.Name;
                    oldTopic.Description = newTopic.Description;
                    oldTopic.EventDate = newTopic.EventDate;
                    oldTopic.UpdateDate = DateTime.Now;

                }
                else
                {
                    return false;
                }

                _context.SaveChanges();
                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return false;
            }
        }
        public bool AddTopicToRecycleBin(int idTopic)
        {
            try
            {
                var topic = _context.Topics.Where(t => t.Id == idTopic).FirstOrDefault();
                if (topic != null)
                {
                    topic.Deletionstatus = 0;
                    _context.SaveChanges();
                    return true;
                }
                else return false;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return false;
            }
        }
        public bool AddTopicAndMathProblemsToRecycleBin(int idTopic)
        {
            try
            {
                var topic = _context.Topics.Include(t => t.MathProblems).Where(t => t.Id == idTopic).FirstOrDefault();
                if (topic != null)
                {
                    topic.Deletionstatus = 0;
                    if(topic.MathProblems != null)
                    {
                        foreach(var mathProblem in topic.MathProblems)
                        {
                            mathProblem.Deletionstatus = 0;
                        }
                    }
                    _context.SaveChanges();
                    return true;
                }
                else return false;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return false;
            }
        }
        public IEnumerable<Topic> GetRecycledTopicsByUser(int idUser)
        {
            var topics = new List<Topic>();
            try
            {
                var IdRole = _context.Users.Where(x => x.Id == idUser).Select(x => x.IdRole).FirstOrDefault();

                if (IdRole == 1)
                {
                    //if the user is Admin select all MathProblems
                    topics = _context.Topics.Where(t => t.Deletionstatus == 0).Select(t => new Topic
                    {
                        Id = t.Id,
                        Name = t.Name,
                        Description = t.Description,
                        Type = t.Type,
                        UpdateDate = t.UpdateDate,
                    }).OrderByDescending(t => t.UpdateDate).ToList();
                }
                else
                {
                    //if the user is a teacher select only their MathProblems
                    topics = _context.Topics.Where(t => t.IdUser == idUser && t.Deletionstatus == 0).Select(t => new Topic
                    {
                        Id = t.Id,
                        Name = t.Name,
                        Description = t.Description,
                        UpdateDate = t.UpdateDate,
                    }).OrderByDescending(t => t.UpdateDate).ToList();
                }
                foreach (var topic in topics)
                {
                    topic.SetUpdatedTimeAgoProperty();
                }
                return topics;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return topics;
            }
        }
        public bool RecoverTopicFromRecycleBin(int idTopic)
        {
            try
            {
                var topic = _context.Topics.Where(t => t.Id == idTopic).FirstOrDefault();
                if (topic != null)
                {
                    topic.Deletionstatus = 1;
                    _context.SaveChanges();
                    return true;
                }
                else return false;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return false;
            }
        }
        public bool RecoverTopicAndMathProblemsFromRecycleBin(int idTopic)
        {
            try
            {
                var topic = _context.Topics.Include(t => t.MathProblems).Where(t => t.Id == idTopic).FirstOrDefault();
                if (topic != null)
                {
                    topic.Deletionstatus = 1;
                    if (topic.MathProblems.Count != 0)
                    {
                        for (int i = 0; i < topic.MathProblems.Count; i++)
                        {
                            topic.MathProblems.ElementAt(i).Deletionstatus = 1;
                            topic.MathProblems.ElementAt(i).Position = topic.MathProblems.Count;
                        }
                    }
                    _context.SaveChanges();
                    return true;
                }
                else return false;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return false;
            }
        }
        public bool DeleteTopic(int idTopic)
        {
            try
            {
                var topic = _context.Topics.Where(t => t.Id == idTopic).FirstOrDefault();
                if (topic != null)
                {
                    _context.Topics.Remove(topic);
                    _context.SaveChanges();
                    return true;
                }
                else return false;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return false;
            }
        }
        public bool DeleteTopicAndMathProblems(int idTopic)
        {
            try
            {
                var topic = _context.Topics.Include(t => t.MathProblems).Where(t => t.Id == idTopic).FirstOrDefault();
                if (topic != null)
                {
                    if (topic.MathProblems.Count != 0)
                    {
                        for (int i = 0; i < topic.MathProblems.Count; i++)
                        {
                            _context.MathProblems.Remove(topic.MathProblems.ElementAt(i));
                        }
                    }
                    _context.Topics.Remove(topic);
                    _context.SaveChanges();
                    return true;
                }
                else return false;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return false;
            }
        }

    }
}
