using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataService
{
    public class Badges:BaseModel
    {
        public Badges(int orgid, string appname, Employee emp)
            : base(orgid, appname,emp)
        {

        }
        // If badgesFrom is null the notification will be shown as readonly with no way to delete
        public List<Badge> GetAllBadgesFor(Guid employeeId)
        {
            using (var dbx = new PEntities())
            {
                var badges = dbx.Badges.Where(b => b.ToBadge == employeeId && !b.Viewed).ToList();
                EvaluationCycle goalCycle = GetGoalCycle();
                EvaluationCycle evalCycle = GetEvalCycle();
                EmployeeEvaluationService e = new EmployeeEvaluationService(OrgId, AppName,CurrentUser);
                if (badges == null)
                    badges = new List<Badge>();


                if (evalCycle == null && PrevEvalCycle!=null)
                {
                    evalCycle = PrevEvalCycle;
                }

                if (evalCycle!=null && e.IsEvaluationComplete(employeeId) && !dbx.FeedbackAnswers.Any(x => x.EmployeeId == employeeId && x.EvalCycleId == evalCycle.Id))
                {
                    badges.Insert(0, new Badge()
                    {
                        BadgeType = BadgeType.GIVE_FEEDBACK,
                        Description = "Please give us your evaluation process feedback. <a href='~/feedback'>Click Here</a>",
                        FromBadge = null,
                        ToBadge = employeeId,
                        Viewed = false
                    });
                }
                evalCycle = GetEvalCycle();
                if (goalCycle != null)
                {
                    Goal g = dbx.Goals.Where(x => x.Evalcycleid == goalCycle.Id && x.EmployeeId == employeeId && !x.Fixed).FirstOrDefault();
                    if (g != null)
                    {
                        if (g.Status < GoalStatus.EMPLOYEE_GOAL_PUBLISHED)
                        {
                            badges.Insert(0, new Badge()
                            {
                                BadgeType = BadgeType.PUBLISH_GOALS,
                                Description = "Publish your saved goals to your manager for approval. <a href='~/goals'>Click Here</a>",
                                FromBadge = null,
                                ToBadge = employeeId,
                                Viewed = false
                            });
                        }
                    }
                    else
                    {
                        badges.Insert(0, new Badge()
                        {
                            BadgeType = BadgeType.BEGIN_GOAL_SETTING,
                            Description = "Please create and set your goals for the current evaluation cycle. <a href='~/goals'>Click Here</a>",
                            FromBadge = null,
                            ToBadge = employeeId,
                            Viewed = false
                        });
                    }
                }
                if (evalCycle != null)
                {
                    Goal g = dbx.Goals.Where(x => !x.Fixed && x.Evalcycleid == evalCycle.Id && x.EmployeeId == employeeId).FirstOrDefault();
                    if (g != null)
                    {
                        if (g.Status < GoalStatus.EMPLOYEE_EVAL_PUBLISHED && g.Status >= GoalStatus.MANAGER_GOAL_PUBLISHED)
                        {
                            badges.Add(new Badge()
                            {
                                BadgeType = BadgeType.PUBLISH_SELF_EVAL,
                                Description = "Publish your self-evaluation to your manager for approval. <a href='~/goals'>Click Here</a>",
                                FromBadge = null,
                                ToBadge = employeeId,
                                Viewed = false
                            });
                        }

                    }
                    else
                    {
                        badges.Add(new Badge()
                        {
                            BadgeType = BadgeType.BEGIN_SELF_EVALUATION,
                            Description = "Please initiate a self-evaluation of your goals for the current evaluation cycle. <a href='~/goals'>Click Here</a>",
                            FromBadge = null,
                            ToBadge = employeeId,
                            Viewed = false
                        });
                    }
                }
                if (badges != null)
                    return badges.ToList();
            }
            return null;
        }

        public List<Badge> GetBadgesOfType(string badgeType, Guid toEmployeeId)
        {
            using (var dbx = new PEntities())
            {
                var badges = dbx.Badges.Where(b => b.ToBadge == toEmployeeId && b.BadgeType == badgeType && !b.Viewed);
                if (badges != null)
                    return badges.ToList();
            }
            return null;
        }


        public List<Badge> GetBadgesFrom(Guid fromEmployeeId, Guid toEmployeeId)
        {
            using (var dbx = new PEntities())
            {
                var badges = dbx.Badges.Where(b => b.ToBadge == toEmployeeId && b.FromBadge == fromEmployeeId && !b.Viewed);
                if (badges != null)
                    return badges.ToList();
            }
            return null;
        }

        
        public bool Delete(long id)
        {
            using (var dbx = new PEntities())
            {
               
                    var bg = dbx.Badges.SingleOrDefault(x => x.Id == id);
                    if (bg != null)
                    {
                        dbx.Badges.Remove(bg);
                        dbx.SaveChanges();
                        return true;
                    }

                    return false;
            }
        }

        public void DeleteBadges(List<long> badges)
        {
            using (var dbx = new PEntities())
            {
                foreach (int b in badges)
                {
                    var bg = dbx.Badges.SingleOrDefault(x => x.Id == b);
                    if (bg != null)
                    {
                        dbx.Badges.Remove(bg);
                        dbx.SaveChanges();
                    }

                }
            }
        }

    }
}
