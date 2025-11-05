namespace Pharmacy.Application.Common.StaticMessages;
public static class Messagies
{
    // ✅ General Success Messages
    public const string Success = "Successfully";
    public const string Created = "Created Successfully";
    public const string Updated = "Updated Successfully";
    public const string Deleted = "Deleted Successfully";

    // ✅ Common Errors
    public const string NotFound = "Not Found";
    public const string Conflicted = "Conflicted Data";

    // ✅ Service-level Messages
    public const string Namealreadyexists = "Name already exists."; 
    public const string GetPagedFailed = "GetPagedFailed";
    public const string GetAllFailed = "Get All Failed";
    public const string GetByIdFailed = "Get By Id Failed";
    public const string GetByNameFailed = "Get By Name Failed";
    public const string FindFailed = "Find Failed";
    public const string CreateFailed = "Create Failed";
    public const string UpdateFailed = "Update Failed";
    public const string DuplicateCategoryName = "Duplicate Category Name";
}
