using OA.Core;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OA.Domain.Inform
{
    [MyTable("inform", null, true, true)]
    public class InformModel
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("phid")]
        public virtual long PhId { get; set; }

        // 编码
        [Column("cno")]
        [MaxLength(60)]
        public virtual string Cno { get; set; }

        // 标题
        [Column("cname")]
        [MaxLength(128)]
        public virtual string Cname { get; set; }

        //所属职能机构
        [Column("deptno")]
        public virtual long Deptno { get; set; }

        //正文
        [Column("cobject", TypeName = "text")]
        public virtual string Cobject { get; set; }
    }
}
