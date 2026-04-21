
namespace RSign.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class Company
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Company()
        {
            this.Domain = new HashSet<Domain>();
            this.UserProfile = new HashSet<UserProfile>();
        }
    
        public System.Guid ID { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
       // public byte[] LogoPath { get; set; }
        public Nullable<System.DateTime> CreatedDate { get; set; }
        public Nullable<System.DateTime> ModifiedDate { get; set; }
        public bool IsActive { get; set; }
        public string? AdminEmailID { get; set; }
        public string? PostSigningLandingPage { get; set; }
        public bool IsTransparencyFeatureOn { get; set; }
        public string? Referencekey { get; set; }
        public bool IsContractToGenerateFromImages { get; set; }
        public Nullable<System.Guid> LanguageID { get; set; }
        public Nullable<int> FinalContractOptionID { get; set; }
        public Nullable<int> ImageOptionID { get; set; }
        public Nullable<bool> IsRSignAPIAccess { get; set; }
        public string? LanguageCode { get; set; }
    
        public virtual Language Language { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Domain> Domain { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<UserProfile> UserProfile { get; set; }
    }
}
