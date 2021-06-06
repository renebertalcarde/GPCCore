using System;

namespace reports
{
    public static class Procs
    {
        //public static Table GetTableByTitle(string title, Tables tables)
        //{
        //    Table ret = null;

        //    foreach (Table table in tables)
        //    {
        //        if (table.Title == title)
        //        {
        //            ret = table;
        //            break;
        //        }
        //        else
        //        {
        //            Table tmp = GetTableByTitle(title, table.Tables);
        //            if (tmp != null)
        //            {
        //                ret = tmp;
        //                break;
        //            }
        //        }
        //    }

        //    return ret;
        //}

        public static void releaseExcelObject(object obj)
        {
            try
            {
                System.Runtime.InteropServices.Marshal.ReleaseComObject(obj);
                obj = null;
            }
            catch (Exception e)
            {
                obj = null;
                throw new Exception("Unable to release the Object " + e.ToString());
            }
            finally
            {
                GC.Collect();
            }
        }
    }
}