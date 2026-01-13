using HOMEOWNER.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HOMEOWNER.Data
{
    /// <summary>
    /// Interface for data access - can be implemented by both EF Core and Firebase
    /// </summary>
    public interface IDataService
    {
        // Homeowners
        IQueryable<Homeowner> Homeowners { get; }
        Task<Homeowner?> GetHomeownerByIdAsync(int id);
        Task<Homeowner?> GetHomeownerByEmailAsync(string email);
        Task AddHomeownerAsync(Homeowner homeowner);
        Task UpdateHomeownerAsync(Homeowner homeowner);
        Task DeleteHomeownerAsync(int id);

        // Admins
        IQueryable<Admin> Admins { get; }
        Task<Admin?> GetAdminByIdAsync(int id);
        Task<Admin?> GetAdminByEmailAsync(string email);
        Task AddAdminAsync(Admin admin);
        Task UpdateAdminAsync(Admin admin);

        // Staff
        IQueryable<Staff> Staff { get; }
        Task<List<Staff>> GetStaffAsync();
        Task<Staff?> GetStaffByIdAsync(int id);
        Task<Staff?> GetStaffByEmailAsync(string email);
        Task<List<Staff>> GetStaffByPositionAsync(string position);
        Task AddStaffAsync(Staff staff);
        Task UpdateStaffAsync(Staff staff);
        Task DeleteStaffAsync(int id);

        // Facilities
        IQueryable<Facility> Facilities { get; }
        Task<Facility?> GetFacilityByIdAsync(int id);
        Task AddFacilityAsync(Facility facility);
        Task UpdateFacilityAsync(Facility facility);
        Task DeleteFacilityAsync(int id);

        // Reservations
        IQueryable<Reservation> Reservations { get; }
        Task<Reservation?> GetReservationByIdAsync(int id);
        Task<List<Reservation>> GetReservationsByHomeownerIdAsync(int homeownerId);
        Task AddReservationAsync(Reservation reservation);
        Task UpdateReservationAsync(Reservation reservation);
        Task DeleteReservationAsync(int id);
        Task<int> SaveChangesAsync();

        // Service Requests
        IQueryable<ServiceRequest> ServiceRequests { get; }
        Task<List<ServiceRequest>> GetServiceRequestsAsync();
        Task<ServiceRequest?> GetServiceRequestByIdAsync(int id);
        Task<List<ServiceRequest>> GetServiceRequestsByHomeownerIdAsync(int homeownerId);
        Task AddServiceRequestAsync(ServiceRequest request);
        Task UpdateServiceRequestAsync(ServiceRequest request);
        Task DeleteServiceRequestAsync(int id);

        // Announcements
        IQueryable<Announcement> Announcements { get; }
        Task<Announcement?> GetAnnouncementByIdAsync(int id);
        Task AddAnnouncementAsync(Announcement announcement);
        Task UpdateAnnouncementAsync(Announcement announcement);

        // Forum Posts
        IQueryable<ForumPost> ForumPosts { get; }
        Task<ForumPost?> GetForumPostByIdAsync(int id);
        Task AddForumPostAsync(ForumPost post);
        Task UpdateForumPostAsync(ForumPost post);

        // Forum Comments
        IQueryable<ForumComment> ForumComments { get; }
        Task AddForumCommentAsync(ForumComment comment);

        // Reactions
        IQueryable<Reaction> Reactions { get; }
        Task AddReactionAsync(Reaction reaction);

        // Events
        IQueryable<EventModel> Events { get; }
        Task<List<EventModel>> GetEventsAsync();
        Task<EventModel?> GetEventByIdAsync(int id);
        Task AddEventAsync(EventModel eventModel);
        Task UpdateEventAsync(EventModel eventModel);
        Task DeleteEventAsync(int id);

        // Community Settings
        Task<CommunitySettings?> GetCommunitySettingsAsync();
        Task AddOrUpdateCommunitySettingsAsync(CommunitySettings settings);

        // Homeowner Profile Images
        IQueryable<HomeownerProfileImage> HomeownerProfileImages { get; }
        Task<HomeownerProfileImage?> GetHomeownerProfileImageAsync(int homeownerId);
        Task AddOrUpdateHomeownerProfileImageAsync(HomeownerProfileImage image);

        // Billing
        IQueryable<Billing> Billings { get; }
        Task<Billing?> GetBillingByIdAsync(int id);
        Task<List<Billing>> GetBillingsByHomeownerIdAsync(int homeownerId);
        Task AddBillingAsync(Billing billing);
        Task UpdateBillingAsync(Billing billing);
        Task DeleteBillingAsync(int id);

        // Documents
        IQueryable<Document> Documents { get; }
        Task<Document?> GetDocumentByIdAsync(int id);
        Task<List<Document>> GetDocumentsByCategoryAsync(string category);
        Task AddDocumentAsync(Document document);
        Task UpdateDocumentAsync(Document document);
        Task DeleteDocumentAsync(int id);
        Task IncrementDownloadCountAsync(int documentId);

        // Contacts
        IQueryable<Contact> Contacts { get; }
        Task<Contact?> GetContactByIdAsync(int id);
        Task<List<Contact>> GetContactsByCategoryAsync(string category);
        Task AddContactAsync(Contact contact);
        Task UpdateContactAsync(Contact contact);
        Task DeleteContactAsync(int id);

        // Visitor Passes
        IQueryable<VisitorPass> VisitorPasses { get; }
        Task<VisitorPass?> GetVisitorPassByIdAsync(int id);
        Task<List<VisitorPass>> GetVisitorPassesByHomeownerIdAsync(int homeownerId);
        Task AddVisitorPassAsync(VisitorPass visitorPass);
        Task UpdateVisitorPassAsync(VisitorPass visitorPass);
        Task DeleteVisitorPassAsync(int id);

        // Vehicle Registration
        IQueryable<VehicleRegistration> VehicleRegistrations { get; }
        Task<VehicleRegistration?> GetVehicleByIdAsync(int id);
        Task<List<VehicleRegistration>> GetVehiclesByHomeownerIdAsync(int homeownerId);
        Task<VehicleRegistration?> GetVehicleByPlateNumberAsync(string plateNumber);
        Task AddVehicleAsync(VehicleRegistration vehicle);
        Task UpdateVehicleAsync(VehicleRegistration vehicle);
        Task DeleteVehicleAsync(int id);

        // Gate Access Logs
        IQueryable<GateAccessLog> GateAccessLogs { get; }
        Task AddGateAccessLogAsync(GateAccessLog log);
        Task<List<GateAccessLog>> GetGateAccessLogsByDateRangeAsync(DateTime startDate, DateTime endDate);

        // Complaints
        IQueryable<Complaint> Complaints { get; }
        Task<Complaint?> GetComplaintByIdAsync(int id);
        Task<List<Complaint>> GetComplaintsByHomeownerIdAsync(int homeownerId);
        Task AddComplaintAsync(Complaint complaint);
        Task UpdateComplaintAsync(Complaint complaint);
        Task DeleteComplaintAsync(int id);

        // Polls
        IQueryable<Poll> Polls { get; }
        Task<Poll?> GetPollByIdAsync(int id);
        Task<List<Poll>> GetActivePollsAsync();
        Task AddPollAsync(Poll poll);
        Task UpdatePollAsync(Poll poll);
        Task DeletePollAsync(int id);
        Task AddPollVoteAsync(PollVote vote);
        Task<bool> HasHomeownerVotedAsync(int pollId, int homeownerId);
    }
}

