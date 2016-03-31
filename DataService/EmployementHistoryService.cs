using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataService
{
    public class EmployementHistoryService
    {
        public List<EmploymentHistory> GetEmploymentHistory(Guid employeeid)
        {
            using(PEntities dbx = new PEntities())
            {
                return dbx.EmploymentHistories.Where(x=>x.EmployeeId==employeeid).OrderByDescending(x=>x.FromDate).ToList();
            }
        }

        public List<EmploymentHistory> AddEmployment(EmploymentHistory employment)
        {
            using (PEntities dbx = new PEntities())
            {
                dbx.EmploymentHistories.Add(employment);
                dbx.SaveChanges();
                return GetEmploymentHistory(employment.EmployeeId);
            }
        }

        public List<EmploymentHistory> UpdateEmployment(EmploymentHistory employment)
        {
            using (PEntities dbx = new PEntities())
            {
                
                dbx.EmploymentHistories.Attach(employment);
                dbx.Entry(employment).State = System.Data.Entity.EntityState.Modified;
                dbx.SaveChanges();
                return GetEmploymentHistory(employment.EmployeeId);
            }
        }

        public List<EmploymentHistory> DeleteEmployment(long id)
        {
            using (PEntities dbx = new PEntities())
            {

                 var emp = dbx.EmploymentHistories.FirstOrDefault(x => x.Id == id);
                 Guid empid = emp.EmployeeId;
                 if (emp != null)
                 {
                     dbx.EmploymentHistories.Remove(emp);
                     dbx.SaveChanges();
                 }
                return GetEmploymentHistory(empid);
            }
        }
    }
    public class EmployeeEducationService{

        public List<EmployeeEducation> GetEmployeeEducation(Guid employeeid)
        {
            using (PEntities dbx = new PEntities())
            {
                return dbx.EmployeeEducations.Where(x => x.EmployeeId == employeeid).OrderByDescending(x => x.YearOfPassing).ToList();
            }
        }

        public List<EmployeeEducation> AddEmployeeEducation(EmployeeEducation education)
        {
            using (PEntities dbx = new PEntities())
            {
                dbx.EmployeeEducations.Add(education);
                dbx.SaveChanges();
                return GetEmployeeEducation(education.EmployeeId);
            }
        }

        public List<EmployeeEducation> UpdateEmployeeEducation(EmployeeEducation education)
        {
            using (PEntities dbx = new PEntities())
            {

                dbx.EmployeeEducations.Attach(education);
                dbx.Entry(education).State = System.Data.Entity.EntityState.Modified;
                dbx.SaveChanges();
                return GetEmployeeEducation(education.EmployeeId);
            }
        }

        public List<EmployeeEducation> DeleteEducation(long id)
        {
            using (PEntities dbx = new PEntities())
            {

                var edu = dbx.EmployeeEducations.FirstOrDefault(x => x.Id == id);
                Guid empid = edu.EmployeeId;
                if (edu != null)
                {
                    dbx.EmployeeEducations.Remove(edu);
                    dbx.SaveChanges();
                }
                return GetEmployeeEducation(empid);
            }
        }
    }

    public class EmployeeSkillService{

        public List<EmployeeSkill> GetEmployeeSkills(Guid employeeid)
        {
            using (PEntities dbx = new PEntities())
            {
                return dbx.EmployeeSkills.Where(x => x.EmployeeId == employeeid).OrderByDescending(x => x.SkillName).ToList();
            }
        }
        public List<EmployeeSkill> AddEmployeeSkill(EmployeeSkill skill)
        {
            using (PEntities dbx = new PEntities())
            {
                dbx.EmployeeSkills.Add(skill);
                dbx.SaveChanges();
                return GetEmployeeSkills(skill.EmployeeId);
            }
        }

        public List<EmployeeSkill> UpdateEmployeeSkill(EmployeeSkill skill)
        {
            using (PEntities dbx = new PEntities())
            {

                dbx.EmployeeSkills.Attach(skill);
                dbx.Entry(skill).State = System.Data.Entity.EntityState.Modified;
                dbx.SaveChanges();
                return GetEmployeeSkills(skill.EmployeeId);
            }
        }

        public List<EmployeeSkill> DeleteEmployeeSkill(long id)
        {
            using (PEntities dbx = new PEntities())
            {

                var skill = dbx.EmployeeSkills.FirstOrDefault(x => x.Id == id);
                Guid empid = skill.EmployeeId;
                if (skill != null)
                {
                    dbx.EmployeeSkills.Remove(skill);
                    dbx.SaveChanges();
                }
                return GetEmployeeSkills(empid);
            }
        }


    }
}
