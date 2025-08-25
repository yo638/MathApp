using MathApp.Models.BusinessModels;
using MathApp.Models.DbModels;
using MathApp.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;

namespace MathApp.Services
{
    public class MathProblemService : IMathProblemService
    {
        private readonly math_appContext _context;

        public MathProblemService(math_appContext context)
        {
            _context = context;
        }

        public bool CreateMathProblem(MathProblem mathProblem)
        {
            try
            {
                _context.MathProblems.Add(mathProblem);
                _context.SaveChanges();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return false;
            }
            return true;
        }
        public IEnumerable<MathProblem> GetMathProblemsByUser(int idUser)
        {
            List<MathProblem> mathProblems = new List<MathProblem>();
            try
            {
                var IdRole = _context.Users.Where(x => x.Id == idUser).Select(x => x.IdRole).FirstOrDefault();

                if (IdRole == 1)
                {
                    //if the user is Admin select all Zadachi
                    mathProblems = _context.MathProblems.Where(z => z.Deletionstatus == 1).Select(z => new MathProblem
                    {
                        Id = z.Id,
                        Conditions = z.Conditions,
                        UpdateDate = z.UpdateDate,
                    }).OrderByDescending(z=>z.UpdateDate)
                    .ToList();
                }
                else
                {
                    //if the user is a teacher select only their Zadachi
                    mathProblems = _context.MathProblems.Where(z => z.IdUser == idUser && z.Deletionstatus == 1).Select(z => new MathProblem
                    {
                        Id = z.Id,
                        Conditions = z.Conditions,
                        UpdateDate = z.UpdateDate,
                    }).OrderByDescending(z => z.UpdateDate)
                    .ToList();
                }
                foreach (var mathProblem in mathProblems)
                {
                    mathProblem.SetUpdatedTimeAgoProperty();
                }
                return mathProblems;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return mathProblems;
            }
        }
        public IEnumerable<MathProblem> GetRecycledMathProblemsByUser(int idUser)
        {
            var mathProblems = new List<MathProblem>();
            try
            {
                var IdRole = _context.Users.Where(x => x.Id == idUser).Select(x => x.IdRole).FirstOrDefault();

                if (IdRole == 1)
                {
                    //if the user is Admin select all Zadachi
                    mathProblems = _context.MathProblems.Where(z => z.Deletionstatus == 0).Select(z => new MathProblem
                    {
                        Id = z.Id,
                        Conditions = z.Conditions,
                        UpdateDate = z.UpdateDate,
                    }).ToList();
                }
                else
                {
                    //if the user is a teacher select only their Zadachi
                    mathProblems = _context.MathProblems.Where(z => z.IdUser == idUser && z.Deletionstatus == 0).Select(z => new MathProblem
                    {
                        Id = z.Id,
                        Conditions = z.Conditions,
                        UpdateDate = z.UpdateDate,
                    }).ToList();
                }
                foreach (var mathProblem in mathProblems)
                {
                    mathProblem.SetUpdatedTimeAgoProperty();
                }
                return mathProblems;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return mathProblems;
            }
        }
        public MathProblem GetMathProblemById(int idMathProblem)
        {
            var mathProblem = new MathProblem();
            try
            {

                mathProblem = _context.MathProblems.Include(z => z.Categories).Include(z => z.Answers).Include(z => z.IdTopicNavigation).Where(z => z.Id == idMathProblem).SingleOrDefault();
                return mathProblem;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return mathProblem;
            }
        }
        public bool UpdateMathProblem(MathProblem newMathProblem)
        {
            try
            {
                // Attach the newMathProblem and mark it as modified
                _context.Attach(newMathProblem);
                _context.Entry(newMathProblem).State = EntityState.Modified;
                _context.SaveChanges();
                // Detach the newMathProblem to reattach with updated collections
                _context.Entry(newMathProblem).State = EntityState.Detached;
                // Fetch the existing mathProblem from the database with its related entities
                var oldMathProblem = _context.MathProblems.Include(z => z.Categories).Include(z => z.Answers)
                                                  .Where(z => z.Id == newMathProblem.Id).SingleOrDefault();
                if (oldMathProblem != null)
                {
                    // Update Answers
                    oldMathProblem.Answers = newMathProblem.Answers;

                    // Update Categories
                    // Clear existing categories
                    oldMathProblem.Categories.Clear();
                    _context.SaveChanges();

                    // Add new categories
                    foreach (var category in newMathProblem.Categories)
                    {
                        oldMathProblem.Categories.Add(category);
                    }
                    _context.SaveChanges();
                }
                else
                {
                    return false;
                }

                // Save all changes
                _context.SaveChanges();

                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return false;
            }
            return true;
        }
        public bool AddMathProblemToRecycleBin(int idMathProblem)
        {
            try
            {
                var mathProblem = _context.MathProblems.FirstOrDefault(e => e.Id == idMathProblem);
                if (mathProblem != null)
                {
                    mathProblem.Deletionstatus = 0;
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
        public bool RecoverMathProblemFromRecycleBin(int idMathProblem)
        {
            try
            {
                var mathProblem = _context.MathProblems.FirstOrDefault(e => e.Id == idMathProblem);
                if (mathProblem != null)
                {
                    mathProblem.Deletionstatus = 1;
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
        public bool DeleteMathProblem(int idMathProblem)
        {
            try
            {
                var mathProblem = _context.MathProblems.FirstOrDefault(z => z.Id == idMathProblem);
                if (mathProblem != null)
                {
                    _context.MathProblems.Remove(mathProblem);
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
        public IEnumerable<MathProblem> SearchMathProblems(int idUser, SearchCriteriaMathProblem criteria)
        {
            var mathProblems = new List<MathProblem>();
            try
            {
                mathProblems = _context.MathProblems.Include(z => z.Categories).Include(z => z.Answers).Where(z => z.IdUser == idUser && z.Deletionstatus == 1).ToList();

                //Searching in Conditions
                if (criteria.uslovie != null) mathProblems = mathProblems.Where(z => z.Conditions.Contains(criteria.uslovie, StringComparison.OrdinalIgnoreCase)).ToList();

                //Searching in Solution 
                if (criteria.solution != null) mathProblems = mathProblems.Where(z => z.Solution.Contains(criteria.solution, StringComparison.OrdinalIgnoreCase)).ToList();

                //Searching in Answers
                if (criteria.answer != null) mathProblems = mathProblems.Where(z => z.Answers.Any(a => a.Name.Contains(criteria.answer, StringComparison.OrdinalIgnoreCase))).ToList();

                //Searching in Conditions AND Solution AND Answers
                if (criteria.anywhere != null) mathProblems = mathProblems.Where(z => z.Conditions.Contains(criteria.anywhere, StringComparison.OrdinalIgnoreCase)
                    || z.Solution.Contains(criteria.anywhere, StringComparison.OrdinalIgnoreCase)
                    || z.Answers.Any(a => a.Name.Contains(criteria.anywhere, StringComparison.OrdinalIgnoreCase))).ToList();

                //Searching in Categories
                if (criteria.category.Grade != 0) mathProblems = mathProblems.Where(z => z.Categories.Any(c => c.Grade == criteria.category.Grade)).ToList();
                if (criteria.category.Difficulty != "X") mathProblems = mathProblems.Where(z => z.Categories.Any(c => c.Difficulty == criteria.category.Difficulty)).ToList();

                //Searching in CreationDate
                if (criteria.fromDate != null) mathProblems = mathProblems.Where(z => z.CreationDate >= DateTime.Parse(criteria.fromDate)).ToList();
                if (criteria.toDate != null) mathProblems = mathProblems.Where(z => z.CreationDate <= DateTime.Parse(criteria.toDate)).ToList();
                foreach (var mathProblem in mathProblems)
                {
                    mathProblem.SetUpdatedTimeAgoProperty();
                }
                return mathProblems;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return mathProblems;
            }
        }
    }
}
