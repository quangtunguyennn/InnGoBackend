namespace InnGoBackend.DTOs.BranchDTOs
{
    public class CreateNewBranchRequestDTO
    {
        public string Name { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public string Phone { get; set; }
        public string Description { get; set; }
        public string ImageURL { get; set; }
    }
}
