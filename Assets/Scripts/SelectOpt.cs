using System;
using UnityEngine;

public class SelectOpt {


	public const int id = -1;
	public static readonly string text;

	public static SelectOpt GetInstanceById(int id, MyCalendar calendar, int todoId = -1){
		switch (id) {
		case PointedDay.id:
			return new PointedDay (calendar.MyDateTime);
		case Memo.id:
			return new Memo ();
		case ThisMonth.id:
			return new ThisMonth (calendar);
		case Previous.id:
			return new Previous ();
		case Future.id:
			return new Future ();
		case All.id:
			return new All ();
		case PointedId.id:
			return new PointedId (todoId);

		}

		return new SelectOpt ();
	}

	SelectOpt(){ 
	}

	public virtual bool IsContain(TodoData td){
		return true;
	}

	public virtual int Sort(TodoData a, TodoData b){
		return a.TodoTime.CompareTo (b.TodoTime);
	}		

	public virtual bool IsMemo(){
		return false;
	}

	public virtual int OptAfterAdd(){
		return PointedDay.id;
	}

	public virtual bool IsNeedUpdateShowInMonthChange(){
		return false;
	}

    public virtual bool EnableAddButton()
    {
        return false;
    }

    public class PointedDay : SelectOpt {

		public new const int id = 0;

		private DateTime pointedDateTime;

		public PointedDay( DateTime pointedDateTime){
			this.pointedDateTime = pointedDateTime;
		}

		public override bool IsContain(TodoData td){
			return IsSameDay(td.TodoTime, pointedDateTime) && !td.IsMemo;
		}

        public override bool EnableAddButton()
        {
            return true;
        }

        bool IsSameDay(DateTime dt1, DateTime dt2){
			return (dt1.Year == dt2.Year && dt1.Month == dt2.Month && dt1.Day == dt2.Day);
		}
	
	}

	public class Memo : SelectOpt {

		public new const int id = 1;

		public override bool IsContain(TodoData td){
			return td.IsMemo;
		}

		public override int Sort(TodoData a, TodoData b){
			return b.LookupTime.CompareTo (a.LookupTime);
		}		

		public override bool IsMemo(){
			return true;
		}

		public override int OptAfterAdd(){
			return Memo.id;
		}

        public override bool EnableAddButton()
        {
            return true;
        }

    }

	public class ThisMonth: SelectOpt {

		public new const int id = 2;

		private MyCalendar calendar;

		public ThisMonth (MyCalendar calendar){
			this.calendar = calendar;
		}

		public override bool IsContain(TodoData td){
			return IsSameMonth(td.TodoTime, calendar.ShowDateTime)
				&& !td.IsMemo;
		}

		bool IsSameMonth(DateTime dt1, DateTime dt2){
			return (dt1.Year == dt2.Year && dt1.Month == dt2.Month);
		}

		public override bool IsNeedUpdateShowInMonthChange(){
			return true;
		}

	}

	public class Previous: SelectOpt {

		public new const int id = 3;

		public override bool IsContain(TodoData td){
			return (td.TodoTime.CompareTo(DateTime.Now) < 0) && !td.IsMemo;
		}

		public override int Sort(TodoData a, TodoData b){
			return b.TodoTime.CompareTo (a.TodoTime);
		}	

	}

	public class Future: SelectOpt {

		public new const int id = 4;

		public override bool IsContain(TodoData td){
			return (td.TodoTime.CompareTo(DateTime.Now) > 0) && !td.IsMemo;
		}

	}

	public class All: SelectOpt {

		public new const int id = 5;

		public override bool IsContain(TodoData td){
			return true;
		}

		public override int Sort(TodoData a, TodoData b){
			return b.LookupTime.CompareTo (a.LookupTime);
		}

	}
		
	public class PointedId: SelectOpt {

		public new const int id = 7;

		private int todoId;

		public PointedId (int todoId){
			this.todoId = todoId;
		}

		public override bool IsContain(TodoData td){
			return td.Id == this.todoId;
		}

	}

}


