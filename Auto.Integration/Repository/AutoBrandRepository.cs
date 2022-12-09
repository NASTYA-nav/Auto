using Auto.Integration.Models;

namespace Auto.Integration.Repository;

/// <summary>
/// Доступ к данным бренда
/// </summary>
public class AutoBrandRepository
{
    /// <summary>
    /// Сервис для доступа к функциям дайнемикса
    /// </summary>
    private readonly ServiceClient _service;

    /// <summary>
    /// Конструктор
    /// </summary>
    public AutoBrandRepository()
    {
        var conn = @"
AuthType=ClientSecret; 
ClientId=5d7d798f-697c-4d75-aca7-4d906932f301; 
Url=https://org350db84c.crm4.dynamics.com/; 
ClientSecret=kaa8Q~HS81~tNKxGF1AcWvEqaTSK5DKiOBUnmaAq;";
        var conn1 = @"
AuthType=OAuth; 
Url=https://org350db84c.crm4.dynamics.com; 
Username=november11822@november11822trialnastya.onmicrosoft.com;
RedirectUri=http://localhost;
AppId=
LoginPrompt=Auto;";
        // torrent it vdn courses
        _service = new ServiceClient(conn1);
        // "AuthType=OAuth;" +
        //     "Url=https://myorg.crm.dynamics.com;" +
        //     "Username=someone@myorg.onmicrosoft.com;" +
        //     "RedirectUri=http://localhost;" +
        //     "AppId=51f81489-12ee-4a9e-aaae-a2591f45987d;" +
        //     "LoginPrompt=Auto"
    }

    /// <summary>
    /// Получение брендов
    /// </summary>
    /// <returns>Список брендов</returns>
    public List<AutoBrand> RetrieveRecords()
    {
        try
        {
            var query = new QueryExpression
            {
                EntityName = "cr34c_brand",
                ColumnSet = new ColumnSet("cr34c_name")
            };

            var info = new List<AutoBrand>();
            var brandRecord = _service.RetrieveMultiple(query);
            if (brandRecord == null || brandRecord.Entities.Count <= 0) return info;
            foreach (var entity in brandRecord.Entities)
            {
                var brandModel = new AutoBrand();

                if (entity.Contains("cr34c_name") && entity["cr34c_name"] != null)
                    brandModel.Name = entity["cr34c_name"].ToString();

                info.Add(brandModel);
            }

            return info;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

    /// <summary>
    /// Уаление Бренда
    /// </summary>
    /// <param name="id"></param>
    public void Delete(Guid id)
    {
        try
        {
            _service.Delete("cr34c_brand", id);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

    /// <summary>
    /// Создание или обновление бренда
    /// </summary>
    /// <param name="brand"> Бренд</param>
    public void Save(AutoBrand brand)
    {
        var brandEntity = new Entity("cr34c_brand");
        
        if (brand.Id != Guid.Empty)
        {
            brandEntity["cr34c_brandid"] = brand.Id;
        }

        brandEntity["cr34c_name"] = brand.Name;

        if (brand.Id == Guid.Empty)
        {
            brand.Id = _service.Create(brandEntity);
        }
        else
        {
            _service.Update(brandEntity);
        }
    }
    
    public AutoBrand GetRecord(Guid brandId)
    {
        var accountModel = new AutoBrand();
       
            var cols = new ColumnSet("cr34c_name");
            var account = _service.Retrieve("cr34c_brand", brandId, cols);
            accountModel.Id = brandId;
            accountModel.Name = account.Attributes["cr34c_name"].ToString();

            return accountModel;
    }
}