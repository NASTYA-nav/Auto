using Auto.Integration.Enums;

namespace Auto.Integration.Models;

/// <summary>
/// Сущьность Бренда
/// </summary>
public class AutoBrand : BaseEntity
{
    /// <summary>
    /// 
    /// </summary>
    [Required]
    [Display(Name = "Название")]
    [StringLength(250, ErrorMessage = "Длина {0} должна быть не менее {2} символов.", MinimumLength = 1)]
    public string? Name { get; set; }

    /// <summary>
    /// 
    /// </summary>
    [Required]
    [Display(Name = "Марка")]
    public EntityReference Brand { get; set; }

    /// <summary>
    /// 
    /// </summary>
    [Required]
    [Display(Name = "Объем двигателя")]
    public int Volume { get; set; }

    /// <summary>
    /// 
    /// </summary>
    [Required]
    [Display(Name = "Комплектация")]
    public string Details { get; set; }

    /// <summary>
    /// 
    /// </summary>
    [Required]
    [Display(Name = "Коробка передач")]
    public TransmissionType TransmissionType { get; set; }

    /// <summary>
    /// 
    /// </summary>
    [Display(Name = "Стоимость")]
    public Money RecommendedAmount { get; set; }

    /// <summary>
    /// 
    /// </summary>
    [Display(Name = "Цвет")]
    public string Color { get; set; }

    /// <summary>
    /// 
    /// </summary>
    [Display(Name = "Год выпуска")]
    public int Year { get; set; }

    /// <summary>
    /// 
    /// </summary>
    public AutoModel AutoModel { get; set; }
}