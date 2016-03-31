using System;
using System.Activities;
using System.Activities.DurableInstancing;
using System.Activities.Hosting;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.DurableInstancing;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace DataWorkFlow
{
    public class WorkflowHostHelper
    {
        WorkflowApplication _wfApp;
        AutoResetEvent _instanceUnloaded = new AutoResetEvent(false);
        bool _iscomplete = false;
        string InstanceStoreConnString { get { return System.Configuration.ConfigurationManager.ConnectionStrings["authconn"].ConnectionString; } }
        public string step = "";
        private bool p;

        private  SqlWorkflowInstanceStore SetupSqlpersistenceStore()
        {
            string connectionString = InstanceStoreConnString;
            SqlWorkflowInstanceStore sqlWFInstanceStore = new SqlWorkflowInstanceStore(connectionString);
            sqlWFInstanceStore.InstanceCompletionAction = InstanceCompletionAction.DeleteAll;
            InstanceHandle handle = sqlWFInstanceStore.CreateInstanceHandle();
            InstanceView view = sqlWFInstanceStore.Execute(handle, new CreateWorkflowOwnerCommand(), TimeSpan.FromSeconds(5));
            handle.Free();
            sqlWFInstanceStore.DefaultInstanceOwner = view.InstanceOwner;
            return sqlWFInstanceStore;
        }

        public WorkflowHostHelper(Dictionary<string, object> ipargs)
        {
            _wfApp = new WorkflowApplication(new GoalSettingActivity(), ipargs);
            _wfApp.InstanceStore = SetupSqlpersistenceStore();

            _wfApp.PersistableIdle = (e) =>
            {
                return PersistableIdleAction.Persist;
            };
            _wfApp.Completed=(e)=>{
                _iscomplete = true;
                _instanceUnloaded.Set();
               
                
            };
            _wfApp.Idle = (e) => {
                _instanceUnloaded.Set();
            };
        }

        public WorkflowHostHelper()
        {
            // TODO: Complete member initialization
            _wfApp = new WorkflowApplication(new GoalSettingActivity());
            _wfApp.InstanceStore = SetupSqlpersistenceStore();

            _wfApp.PersistableIdle = (e) =>
            {
                return PersistableIdleAction.Persist;
            };
            _wfApp.Completed = (e) =>
            {
                _iscomplete = true;
                _instanceUnloaded.Set();


            };
            _wfApp.Idle = (e) =>
            {
                _instanceUnloaded.Set();
            };
        }

        public Guid StartWF()
        {
            _wfApp.Run();
            _instanceUnloaded.WaitOne();
            System.Collections.ObjectModel.ReadOnlyCollection<BookmarkInfo> bookmarks = _wfApp.GetBookmarks();
            if (bookmarks != null && bookmarks.Count > 0)
            {
                step = bookmarks[0].BookmarkName;
            }
            

            return _wfApp.Id;
        }


        public void Unload()
        {
            if (_wfApp != null) {
                _wfApp.Unload();
            }
        }

        public string ResumeWF(Guid guid)
        {
            _wfApp.Load(guid);
            string bookmark = "GoalsSet";

            System.Collections.ObjectModel.ReadOnlyCollection<BookmarkInfo> bookmarks = _wfApp.GetBookmarks();
            if (bookmarks != null && bookmarks.Count > 0)
            {
                step=bookmark = bookmarks[0].BookmarkName;
            }
            return bookmark;
        }

        public string RunWF(string cmd)
        {
            string bookmarkName = _wfApp.GetBookmarks()[0].BookmarkName;
            _wfApp.ResumeBookmark(bookmarkName, cmd);
            _instanceUnloaded.WaitOne();
            
            if (!_iscomplete)
            {
                bookmarkName = _wfApp.GetBookmarks()[0].BookmarkName;
                return bookmarkName;
            }
            else
                return "GoalsSet";
        }

    }
}
