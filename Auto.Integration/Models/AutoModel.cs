namespace Auto.Integration.Models;

/// <summary>
/// Сущьность Модели
/// </summary>
public class AutoModel: BaseEntity
{

    /// <summary>
    /// 
    /// </summary>
    [Required]
    [Display(Name = "Название")]
    [StringLength(250, ErrorMessage = "Длина {0} должна быть не менее {2} символов.", MinimumLength = 1)]
    public string Name { get; set; }
}