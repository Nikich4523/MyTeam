//------------------------------------------------------------------------------
// <auto-generated>
//     Этот код создан по шаблону.
//
//     Изменения, вносимые в этот файл вручную, могут привести к непредвиденной работе приложения.
//     Изменения, вносимые в этот файл вручную, будут перезаписаны при повторном создании кода.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Курсовой_проект_3._1
{
    using System;
    using System.Collections.Generic;
    
    public partial class Matches
    {
        public int Id { get; set; }
        public int FK_FrstTeam_Id { get; set; }
        public int FK_ScndTeam_Id { get; set; }
        public int FrstScore { get; set; }
        public int ScndScore { get; set; }
        public Nullable<int> FK_WinnerTeam_Id { get; set; }
        public System.DateTime GameDate { get; set; }
        public int RoundNum { get; set; }
        public int FK_Tournament_Id { get; set; }
    
        public virtual Tournaments Tournaments { get; set; }
        public virtual Teams Teams { get; set; }
        public virtual Teams Teams1 { get; set; }
        public virtual Teams Teams2 { get; set; }
    }
}
