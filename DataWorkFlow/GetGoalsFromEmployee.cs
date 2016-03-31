using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Activities;

namespace DataWorkFlow
{

    public sealed class GetGoalsFromEmployee<T> : NativeActivity<T>
    {
        // Define an activity input argument of type string
        public InArgument<string> Text { get; set; }
        public InArgument<string> BookmarkName { get; set; }
        public OutArgument<string> waViewName { get; set; }


        // If your activity returns a value, derive from CodeActivity<TResult>
        // and return the value from the Execute method.
        protected override void Execute(NativeActivityContext context)
        {
            string bookmarkName = context.GetValue(this.BookmarkName);
            context.CreateBookmark(bookmarkName, new BookmarkCallback(this.Continue));
            // Obtain the runtime value of the Text input argument
            string text = context.GetValue(this.Text);
            waViewName.Set(context, "Goals");
        }
        private void Continue(NativeActivityContext context, Bookmark bookmark, object value)
        {
            waViewName.Set(context, (T)value);
        }
        protected override bool CanInduceIdle
        {
            get
            {
                return true;
            }
        }
    }
}
