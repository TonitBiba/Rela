//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Rela_project.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class GoogleFace
    {
        public int googleId { get; set; }
        public Nullable<int> imageId { get; set; }
        public string GoogleFace1 { get; set; }
    
        public virtual ImagesProcesed ImagesProcesed { get; set; }
    }
}
