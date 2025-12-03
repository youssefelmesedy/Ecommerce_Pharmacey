namespace Pharmacy.Application.Common.StaticMessages;
public static class Messagies
{
    // ✅ General Success Messages
    public const string Success = "Successfully";
    public const string Created = "Created Successfully";
    public const string Updated = "Updated Successfully";
    public const string Deleted = "Deleted Successfully";
    // Failed Constans
    public const string CreateFailed = "Create Failed";
    public const string UpdateFailed = "Update Failed";
    public const string DeleteFailed = "Delete Failed";

    public const string GetPagination = "Retrieved Pagination Successfully";
    public const string GetAllFailed = "Retrieved All Failed";
    public const string GetByIdFailed = "Retrieved By Id Failed";
    public const string GetByNameFailed = "Retrieved By Name Failed";
    public const string FindFailed = "Find Failed";

    // ✅ Common Errors
    public const string NotFound = "Not Found";
    public const string Conflicted = "Conflicted Data";
    public const string MappingFailed = "The Mapping Failed";
    public const string DuplicateCategoryName = "Duplicate Category Name";
    public const string Namealreadyexists = "Name already exists."; 

    // ✅ Service-level Messages
    public const string GetPagedFailed = "Retrieved Pagination Failed";
    public const string GetAll = "Retrieved All Successfully";
    public const string GetById = "Retrieved By ID Successfully";

    // Massegies Invalid
    public const string InvalidProduct_ID = "Invalid Product Id";
    public const string InvalidCategory_ID = "Invalid Category Id";
}
