using MathApp.Models.BusinessModels;
using MathApp.Models.DbModels;
using System.Collections.Generic;

namespace MathApp.Services.Interfaces
{
    public interface IMathProblemService
    {
        bool CreateMathProblem(MathProblem zadacha);
        IEnumerable<MathProblem> GetMathProblemsByUser(int idUser);
        IEnumerable<MathProblem> GetRecycledMathProblemsByUser(int idUser);
        MathProblem GetMathProblemById(int idZadacha);
        bool UpdateMathProblem(MathProblem newZadacha);
        bool AddMathProblemToRecycleBin(int idZadacha);
        bool RecoverMathProblemFromRecycleBin(int idZadacha);
        bool DeleteMathProblem(int idZadacha);
        IEnumerable<MathProblem> SearchMathProblems(int idUser, SearchCriteriaMathProblem criteria);

    }
}
