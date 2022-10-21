using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OA.Core
{
    public interface DbStructure
    {
    }

    public class TableDefi : DbStructure
    {
        public int ObjectId { get; set; }
        public string Name { get; set; }
        public List<ColumnDefi> Columns { get; set; }
        public List<IndexDefi> Indexs { get; set; }
        public List<DefaultConstraintDefi> DefaultConstraints { get; set; }
        public IdentityDefi Identity { get; internal set; }

        public string ToCreateSqlSqlServer()
        {
            string template = "create table {0}(\r\n{1}\r\n)";
            List<string> list_cols = new List<string>();
            for (int i = 0; i < Columns.Count; i++)
            {
                var c = Columns[i];
                string tcolstrsql = $"{c.Name} {GetTypeName(c)} {(c.Nullable ? "NULL" : "NOT NULL")} {(c.IsIdentity ? $"identity({c.Identity.send},{c.Identity.crement})" : "")}";
                if (c.DefaualtConstraint != null)
                {
                    tcolstrsql += $" default ({c.DefaualtConstraint.Definition})";
                }
                list_cols.Add(tcolstrsql);
            }
            foreach (var id in Indexs)
            {
                if (id.IsPrimaryKey)
                {
                    list_cols.Add($"constraint {id.Name} primary key ({string.Join(",", id.Columns.Select(cc => cc.Column.Name))})");
                }
                else
                {
                    list_cols.Add($"index {id.Name} {id.TypeDesc} ({string.Join(",", id.Columns.Select(cc => cc.Column.Name))})");
                }
            }

            var rx = string.Format(template, Name, string.Join(",\r\n", list_cols)); ;
            return rx;
        }

        public string ToCreateSqlSqlServerAddDefault()
        {
            List<string> list_cols = new List<string>();
            for (int i = 0; i < Columns.Count; i++)
            {
                var c = Columns[i];
                if (c.DefaualtConstraint == null)
                    continue;


                string tcolstrsql = $"if not exists (select name from sys.default_constraints where name='{c.DefaualtConstraint.Name}') alter table [{Name}] add constraint [{c.DefaualtConstraint.Name}] default ({c.DefaualtConstraint.Definition}) for {c.Name};";
                list_cols.Add(tcolstrsql);
            }
            var rx = string.Join("\r\n", list_cols);
            return rx;
        }
        private string GetTypeName(ColumnDefi col)
        {
            if (col.Type == "text" || col.Type == "datetime" || col.Type == "int" || col.Type == "bigint" ||
                col.Type == "image" || col.Type == "ntext" || col.Type == "bit"
                )
            {
                return string.Format("{0}", col.Type);
            }
            if (col.MaxLength == 0)
            {
                return string.Format("{0}", col.Type);
            }
            if (col.MaxLength == -1)
            {
                return string.Format("{0}(MAX)", col.Type);
            }
            else
            {
                if (col.Scale == 0)
                {
                    return string.Format("{0}({1})", col.Type, col.MaxLength);
                }
                else
                {
                    return string.Format("{0}({1},{2})", col.Type, col.Precision, col.Scale);
                }
            }
        }


    }

    public class DefaultConstraintDefi : DbStructure
    {

        public string Name { get; set; }
        public int ObjectId { get; set; }
        public int ParentObjectId { get; set; }
        public int ColumnId { get; set; }
        public string Definition { get; set; }
        public ColumnDefi Column { get; set; }
        public TableDefi Table { get; set; }
    }

    public class ColumnDefi : DbStructure
    {
        public string Name { get; set; }
        public int ObjectId { get; set; }
        public int ColumnId { get; set; }
        public int TypeId { get; set; }
        public string Type { get; set; }
        public int MaxLength { get; set; }
        public int Precision { get; set; }
        public int Scale { get; set; }
        public bool Nullable { get; set; }
        public bool IsIdentity { get; set; }
        public bool IsPrimaryKey { get; set; }
        public List<IndexDefi> Indexs { get; set; }
        public DefaultConstraintDefi DefaualtConstraint { get; set; }
        public TableDefi Table { get; set; }
        public IdentityDefi Identity { get; set; }
    }

    public class IndexDefi : DbStructure
    {
        public int IndexId { get; set; }
        public int ObjectId { get; set; }
        public string Name { get; set; }
        public bool IsUnique { get; set; }
        public bool IsPrimaryKey { get; set; }
        public bool IsUniqueConstraint { get; set; }
        public int Type { get; set; }
        public string TypeDesc { get; set; }
        public List<IndexColumnDefi> Columns { get; set; }
        public TableDefi Table { get; set; }
    }

    public class IndexColumnDefi : DbStructure
    {
        public IndexDefi Index { get; set; }
        public int IndexId { get; set; }
        public int ObjectId { get; set; }
        public int Ordinal { get; set; }
        public int ColumnId { get; set; }
        public bool IsDescending { get; set; }
        public bool IsIncluded { get; set; }
        public ColumnDefi Column { get; set; }
    }

    public class IdentityDefi : DbStructure
    {
        public ColumnDefi column { get; set; }
        public long send { get; set; }
        public long crement { get; set; }
        public long lastvalue { get; set; }
    }
}
