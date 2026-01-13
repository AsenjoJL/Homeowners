using Google.Cloud.Firestore;
using HOMEOWNER.Data;
using HOMEOWNER.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HOMEOWNER.Services
{
    public class FirebaseService : IDataService
    {
        private readonly FirestoreDb _db;

        public FirebaseService()
        {
            try
            {
                // Initialize Firestore with project ID
                _db = FirestoreDb.Create("homeowner-c355d");
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException(
                    "Firebase credentials not found. Please set GOOGLE_APPLICATION_CREDENTIALS environment variable " +
                    "to point to your Firebase service account JSON file. " +
                    "See HOW_TO_RUN.md for instructions. Error: " + ex.Message, ex);
            }
        }

        // Collections
        private CollectionReference HomeownersCollection => _db.Collection("homeowners");
        private CollectionReference AdminsCollection => _db.Collection("admins");
        private CollectionReference StaffCollection => _db.Collection("staff");
        private CollectionReference FacilitiesCollection => _db.Collection("facilities");
        private CollectionReference ReservationsCollection => _db.Collection("reservations");
        private CollectionReference ServiceRequestsCollection => _db.Collection("serviceRequests");
        private CollectionReference AnnouncementsCollection => _db.Collection("announcements");
        private CollectionReference ForumPostsCollection => _db.Collection("forumPosts");
        private CollectionReference ForumCommentsCollection => _db.Collection("forumComments");
        private CollectionReference ReactionsCollection => _db.Collection("reactions");
        private CollectionReference EventsCollection => _db.Collection("events");
        private CollectionReference NotificationsCollection => _db.Collection("notifications");
        private CollectionReference CommunitySettingsCollection => _db.Collection("communitySettings");
        private CollectionReference HomeownerProfileImagesCollection => _db.Collection("homeownerProfileImages");
        private CollectionReference BillingsCollection => _db.Collection("billings");
        private CollectionReference DocumentsCollection => _db.Collection("documents");
        private CollectionReference ContactsCollection => _db.Collection("contacts");
        private CollectionReference VisitorPassesCollection => _db.Collection("visitorPasses");
        private CollectionReference VehicleRegistrationsCollection => _db.Collection("vehicleRegistrations");
        private CollectionReference GateAccessLogsCollection => _db.Collection("gateAccessLogs");
        private CollectionReference ComplaintsCollection => _db.Collection("complaints");
        private CollectionReference PollsCollection => _db.Collection("polls");
        private CollectionReference PollOptionsCollection => _db.Collection("pollOptions");
        private CollectionReference PollVotesCollection => _db.Collection("pollVotes");

        // Homeowners
        public async Task<List<Homeowner>> GetHomeownersAsync()
        {
            var snapshot = await HomeownersCollection.GetSnapshotAsync();
            return snapshot.Documents.Select(doc => doc.ConvertTo<Homeowner>()).ToList();
        }

        public async Task<Homeowner?> GetHomeownerByIdAsync(int id)
        {
            var doc = await HomeownersCollection.Document(id.ToString()).GetSnapshotAsync();
            return doc.Exists ? doc.ConvertTo<Homeowner>() : null;
        }

        public async Task<Homeowner?> GetHomeownerByEmailAsync(string email)
        {
            var query = HomeownersCollection.WhereEqualTo("Email", email);
            var snapshot = await query.GetSnapshotAsync();
            return snapshot.Documents.FirstOrDefault()?.ConvertTo<Homeowner>();
        }

        public async Task AddHomeownerAsync(Homeowner homeowner)
        {
            // Ensure CreatedAt is UTC for Firestore
            if (homeowner.CreatedAt.Kind != DateTimeKind.Utc)
            {
                homeowner.CreatedAt = homeowner.CreatedAt.ToUniversalTime();
            }
            await HomeownersCollection.Document(homeowner.HomeownerID.ToString()).SetAsync(homeowner);
        }

        public async Task UpdateHomeownerAsync(Homeowner homeowner)
        {
            await HomeownersCollection.Document(homeowner.HomeownerID.ToString()).SetAsync(homeowner);
        }

        public async Task DeleteHomeownerAsync(int id)
        {
            await HomeownersCollection.Document(id.ToString()).DeleteAsync();
        }

        public async Task<int> GetHomeownerCountAsync(string role = "Homeowner")
        {
            var query = HomeownersCollection.WhereEqualTo("Role", role);
            var snapshot = await query.GetSnapshotAsync();
            return snapshot.Count;
        }

        // Admins
        public async Task<List<Admin>> GetAdminsAsync()
        {
            var snapshot = await AdminsCollection.GetSnapshotAsync();
            return snapshot.Documents.Select(doc => doc.ConvertTo<Admin>()).ToList();
        }

        public async Task<Admin?> GetAdminByIdAsync(int id)
        {
            var doc = await AdminsCollection.Document(id.ToString()).GetSnapshotAsync();
            return doc.Exists ? doc.ConvertTo<Admin>() : null;
        }

        public async Task<Admin?> GetAdminByEmailAsync(string email)
        {
            var query = AdminsCollection.WhereEqualTo("Email", email);
            var snapshot = await query.GetSnapshotAsync();
            return snapshot.Documents.FirstOrDefault()?.ConvertTo<Admin>();
        }

        public async Task AddAdminAsync(Admin admin)
        {
            await AdminsCollection.Document(admin.AdminID.ToString()).SetAsync(admin);
        }

        public async Task UpdateAdminAsync(Admin admin)
        {
            await AdminsCollection.Document(admin.AdminID.ToString()).SetAsync(admin);
        }

        // Staff
        public async Task<List<Staff>> GetStaffAsync()
        {
            var snapshot = await StaffCollection.GetSnapshotAsync();
            return snapshot.Documents.Select(doc => doc.ConvertTo<Staff>()).ToList();
        }

        public async Task<Staff?> GetStaffByIdAsync(int id)
        {
            var doc = await StaffCollection.Document(id.ToString()).GetSnapshotAsync();
            return doc.Exists ? doc.ConvertTo<Staff>() : null;
        }

        public async Task<Staff?> GetStaffByEmailAsync(string email)
        {
            var query = StaffCollection.WhereEqualTo("Email", email);
            var snapshot = await query.GetSnapshotAsync();
            return snapshot.Documents.FirstOrDefault()?.ConvertTo<Staff>();
        }

        public async Task<List<Staff>> GetStaffByPositionAsync(string position)
        {
            var query = StaffCollection.WhereEqualTo("Position", position);
            var snapshot = await query.GetSnapshotAsync();
            return snapshot.Documents.Select(doc => doc.ConvertTo<Staff>()).ToList();
        }

        public async Task AddStaffAsync(Staff staff)
        {
            // Generate StaffID if not set
            if (staff.StaffID == 0)
            {
                var allStaff = await GetStaffAsync();
                staff.StaffID = allStaff.Any() ? allStaff.Max(s => s.StaffID) + 1 : 1;
            }

            // Ensure CreatedAt is UTC for Firestore
            if (staff.CreatedAt == default(DateTime) || staff.CreatedAt.Kind != DateTimeKind.Utc)
            {
                staff.CreatedAt = DateTime.UtcNow;
            }

            await StaffCollection.Document(staff.StaffID.ToString()).SetAsync(staff);
        }

        public async Task UpdateStaffAsync(Staff staff)
        {
            await StaffCollection.Document(staff.StaffID.ToString()).SetAsync(staff);
        }

        public async Task DeleteStaffAsync(int id)
        {
            await StaffCollection.Document(id.ToString()).DeleteAsync();
        }

        public async Task<int> GetStaffCountAsync()
        {
            var snapshot = await StaffCollection.GetSnapshotAsync();
            return snapshot.Count;
        }

        // Facilities
        public async Task<List<Facility>> GetFacilitiesAsync()
        {
            var snapshot = await FacilitiesCollection.GetSnapshotAsync();
            return snapshot.Documents.Select(doc => doc.ConvertTo<Facility>()).ToList();
        }

        public async Task<List<Facility>> GetAvailableFacilitiesAsync()
        {
            var query = FacilitiesCollection.WhereEqualTo("AvailabilityStatus", "Available");
            var snapshot = await query.GetSnapshotAsync();
            return snapshot.Documents.Select(doc => doc.ConvertTo<Facility>()).ToList();
        }

        public async Task<Facility?> GetFacilityByIdAsync(int id)
        {
            var doc = await FacilitiesCollection.Document(id.ToString()).GetSnapshotAsync();
            return doc.Exists ? doc.ConvertTo<Facility>() : null;
        }

        public async Task AddFacilityAsync(Facility facility)
        {
            // Auto-generate FacilityID if not set
            if (facility.FacilityID == 0)
            {
                var allFacilities = await GetFacilitiesAsync();
                facility.FacilityID = allFacilities.Any() ? allFacilities.Max(f => f.FacilityID) + 1 : 1;
            }
            await FacilitiesCollection.Document(facility.FacilityID.ToString()).SetAsync(facility);
        }

        public async Task UpdateFacilityAsync(Facility facility)
        {
            await FacilitiesCollection.Document(facility.FacilityID.ToString()).SetAsync(facility);
        }

        public async Task DeleteFacilityAsync(int id)
        {
            await FacilitiesCollection.Document(id.ToString()).DeleteAsync();
        }

        // Reservations
        public async Task<List<Reservation>> GetReservationsAsync()
        {
            var snapshot = await ReservationsCollection.GetSnapshotAsync();
            return snapshot.Documents.Select(doc => doc.ConvertTo<Reservation>()).ToList();
        }

        public async Task<List<Reservation>> GetReservationsByHomeownerIdAsync(int homeownerId)
        {
            var query = ReservationsCollection.WhereEqualTo("HomeownerID", homeownerId);
            var snapshot = await query.GetSnapshotAsync();
            return snapshot.Documents.Select(doc => doc.ConvertTo<Reservation>()).ToList();
        }

        public async Task<List<Reservation>> GetReservationsByStatusAsync(string status)
        {
            var query = ReservationsCollection.WhereEqualTo("Status", status);
            var snapshot = await query.GetSnapshotAsync();
            return snapshot.Documents.Select(doc => doc.ConvertTo<Reservation>()).ToList();
        }

        public async Task<Reservation?> GetReservationByIdAsync(int id)
        {
            var doc = await ReservationsCollection.Document(id.ToString()).GetSnapshotAsync();
            return doc.Exists ? doc.ConvertTo<Reservation>() : null;
        }

        public async Task AddReservationAsync(Reservation reservation)
        {
            await ReservationsCollection.Document(reservation.ReservationID.ToString()).SetAsync(reservation);
        }

        public async Task UpdateReservationAsync(Reservation reservation)
        {
            await ReservationsCollection.Document(reservation.ReservationID.ToString()).SetAsync(reservation);
        }

        public async Task DeleteReservationAsync(int id)
        {
            await ReservationsCollection.Document(id.ToString()).DeleteAsync();
        }

        public async Task<int> GetReservationCountByStatusAsync(string status)
        {
            var query = ReservationsCollection.WhereEqualTo("Status", status);
            var snapshot = await query.GetSnapshotAsync();
            return snapshot.Count;
        }

        // Service Requests
        public async Task<List<ServiceRequest>> GetServiceRequestsAsync()
        {
            var snapshot = await ServiceRequestsCollection.GetSnapshotAsync();
            return snapshot.Documents.Select(doc => doc.ConvertTo<ServiceRequest>()).ToList();
        }

        public async Task<List<ServiceRequest>> GetServiceRequestsByHomeownerIdAsync(int homeownerId)
        {
            var query = ServiceRequestsCollection.WhereEqualTo("HomeownerID", homeownerId);
            var snapshot = await query.GetSnapshotAsync();
            return snapshot.Documents.Select(doc => doc.ConvertTo<ServiceRequest>()).ToList();
        }

        public async Task<List<ServiceRequest>> GetServiceRequestsByStatusAsync(string status)
        {
            var query = ServiceRequestsCollection.WhereEqualTo("Status", status);
            var snapshot = await query.GetSnapshotAsync();
            return snapshot.Documents.Select(doc => doc.ConvertTo<ServiceRequest>()).ToList();
        }

        public async Task<List<ServiceRequest>> GetServiceRequestsByCategoryAsync(string category)
        {
            var query = ServiceRequestsCollection.WhereEqualTo("Category", category);
            var snapshot = await query.GetSnapshotAsync();
            return snapshot.Documents.Select(doc => doc.ConvertTo<ServiceRequest>()).ToList();
        }

        public async Task<ServiceRequest?> GetServiceRequestByIdAsync(int id)
        {
            var doc = await ServiceRequestsCollection.Document(id.ToString()).GetSnapshotAsync();
            return doc.Exists ? doc.ConvertTo<ServiceRequest>() : null;
        }

        public async Task AddServiceRequestAsync(ServiceRequest request)
        {
            await ServiceRequestsCollection.Document(request.RequestID.ToString()).SetAsync(request);
        }

        public async Task UpdateServiceRequestAsync(ServiceRequest request)
        {
            await ServiceRequestsCollection.Document(request.RequestID.ToString()).SetAsync(request);
        }

        public async Task DeleteServiceRequestAsync(int id)
        {
            await ServiceRequestsCollection.Document(id.ToString()).DeleteAsync();
        }

        // Announcements
        public async Task<List<Announcement>> GetAnnouncementsAsync()
        {
            var snapshot = await AnnouncementsCollection.GetSnapshotAsync();
            return snapshot.Documents.Select(doc => doc.ConvertTo<Announcement>()).OrderByDescending(a => a.PostedAt).ToList();
        }

        public async Task<Announcement?> GetAnnouncementByIdAsync(int id)
        {
            var doc = await AnnouncementsCollection.Document(id.ToString()).GetSnapshotAsync();
            return doc.Exists ? doc.ConvertTo<Announcement>() : null;
        }

        public async Task AddAnnouncementAsync(Announcement announcement)
        {
            await AnnouncementsCollection.Document(announcement.AnnouncementID.ToString()).SetAsync(announcement);
        }

        public async Task UpdateAnnouncementAsync(Announcement announcement)
        {
            await AnnouncementsCollection.Document(announcement.AnnouncementID.ToString()).SetAsync(announcement);
        }

        public async Task DeleteAnnouncementAsync(int id)
        {
            await AnnouncementsCollection.Document(id.ToString()).DeleteAsync();
        }

        // Forum Posts
        public async Task<List<ForumPost>> GetForumPostsAsync()
        {
            var snapshot = await ForumPostsCollection.GetSnapshotAsync();
            var posts = snapshot.Documents.Select(doc => doc.ConvertTo<ForumPost>()).ToList();
            
            // Load related data
            foreach (var post in posts)
            {
                // Load comments
                var commentsQuery = ForumCommentsCollection.WhereEqualTo("ForumPostID", post.ForumPostID);
                var commentsSnapshot = await commentsQuery.GetSnapshotAsync();
                post.Comments = commentsSnapshot.Documents.Select(doc => doc.ConvertTo<ForumComment>()).ToList();

                // Load reactions
                var reactionsQuery = ReactionsCollection.WhereEqualTo("ForumPostID", post.ForumPostID);
                var reactionsSnapshot = await reactionsQuery.GetSnapshotAsync();
                post.Reactions = reactionsSnapshot.Documents.Select(doc => doc.ConvertTo<Reaction>()).ToList();
            }

            return posts.OrderByDescending(p => p.CreatedAt).ToList();
        }

        public async Task<ForumPost?> GetForumPostByIdAsync(int id)
        {
            var doc = await ForumPostsCollection.Document(id.ToString()).GetSnapshotAsync();
            if (!doc.Exists) return null;

            var post = doc.ConvertTo<ForumPost>();
            
            // Load comments
            var commentsQuery = ForumCommentsCollection.WhereEqualTo("ForumPostID", post.ForumPostID);
            var commentsSnapshot = await commentsQuery.GetSnapshotAsync();
            post.Comments = commentsSnapshot.Documents.Select(doc => doc.ConvertTo<ForumComment>()).ToList();

            // Load reactions
            var reactionsQuery = ReactionsCollection.WhereEqualTo("ForumPostID", post.ForumPostID);
            var reactionsSnapshot = await reactionsQuery.GetSnapshotAsync();
            post.Reactions = reactionsSnapshot.Documents.Select(doc => doc.ConvertTo<Reaction>()).ToList();

            return post;
        }

        public async Task AddForumPostAsync(ForumPost post)
        {
            await ForumPostsCollection.Document(post.ForumPostID.ToString()).SetAsync(post);
        }

        public async Task UpdateForumPostAsync(ForumPost post)
        {
            await ForumPostsCollection.Document(post.ForumPostID.ToString()).SetAsync(post);
        }

        public async Task DeleteForumPostAsync(int id)
        {
            await ForumPostsCollection.Document(id.ToString()).DeleteAsync();
        }

        // Forum Comments
        public async Task AddForumCommentAsync(ForumComment comment)
        {
            await ForumCommentsCollection.Document(comment.ForumCommentID.ToString()).SetAsync(comment);
        }

        public async Task DeleteForumCommentAsync(int id)
        {
            await ForumCommentsCollection.Document(id.ToString()).DeleteAsync();
        }

        // Reactions
        public async Task AddReactionAsync(Reaction reaction)
        {
            await ReactionsCollection.Document(reaction.ReactionID.ToString()).SetAsync(reaction);
        }

        public async Task DeleteReactionAsync(int id)
        {
            await ReactionsCollection.Document(id.ToString()).DeleteAsync();
        }

        // Events
        public async Task<List<EventModel>> GetEventsAsync()
        {
            var snapshot = await EventsCollection.GetSnapshotAsync();
            return snapshot.Documents.Select(doc => doc.ConvertTo<EventModel>()).OrderBy(e => e.EventDate).ToList();
        }

        public async Task<EventModel?> GetEventByIdAsync(int id)
        {
            var doc = await EventsCollection.Document(id.ToString()).GetSnapshotAsync();
            return doc.Exists ? doc.ConvertTo<EventModel>() : null;
        }

        public async Task AddEventAsync(EventModel eventModel)
        {
            // Auto-generate EventID if not set
            if (eventModel.EventID == 0)
            {
                var allEvents = await GetEventsAsync();
                eventModel.EventID = allEvents.Any() ? allEvents.Max(e => e.EventID) + 1 : 1;
            }
            await EventsCollection.Document(eventModel.EventID.ToString()).SetAsync(eventModel);
        }

        public async Task UpdateEventAsync(EventModel eventModel)
        {
            await EventsCollection.Document(eventModel.EventID.ToString()).SetAsync(eventModel);
        }

        public async Task DeleteEventAsync(int id)
        {
            await EventsCollection.Document(id.ToString()).DeleteAsync();
        }

        // Community Settings
        public async Task<CommunitySettings?> GetCommunitySettingsAsync()
        {
            var snapshot = await CommunitySettingsCollection.GetSnapshotAsync();
            return snapshot.Documents.FirstOrDefault()?.ConvertTo<CommunitySettings>();
        }

        public async Task AddOrUpdateCommunitySettingsAsync(CommunitySettings settings)
        {
            await CommunitySettingsCollection.Document("settings").SetAsync(settings);
        }

        // Homeowner Profile Images
        public async Task<HomeownerProfileImage?> GetHomeownerProfileImageAsync(int homeownerId)
        {
            var query = HomeownerProfileImagesCollection.WhereEqualTo("HomeownerID", homeownerId);
            var snapshot = await query.GetSnapshotAsync();
            return snapshot.Documents.FirstOrDefault()?.ConvertTo<HomeownerProfileImage>();
        }

        public async Task AddOrUpdateHomeownerProfileImageAsync(HomeownerProfileImage image)
        {
            var query = HomeownerProfileImagesCollection.WhereEqualTo("HomeownerID", image.HomeownerID);
            var snapshot = await query.GetSnapshotAsync();
            var existingDoc = snapshot.Documents.FirstOrDefault();

            if (existingDoc != null)
            {
                await existingDoc.Reference.SetAsync(image);
            }
            else
            {
                await HomeownerProfileImagesCollection.Document().SetAsync(image);
            }
        }

        // Notifications
        public async Task<List<Notification>> GetNotificationsAsync()
        {
            var snapshot = await NotificationsCollection.GetSnapshotAsync();
            return snapshot.Documents.Select(doc => doc.ConvertTo<Notification>()).ToList();
        }

        public async Task AddNotificationAsync(Notification notification)
        {
            await NotificationsCollection.Document().SetAsync(notification);
        }

        // IDataService Implementation - IQueryable properties
        public IQueryable<Homeowner> Homeowners => GetHomeownersAsync().GetAwaiter().GetResult().AsQueryable();
        public IQueryable<Admin> Admins => GetAdminsAsync().GetAwaiter().GetResult().AsQueryable();
        public IQueryable<Staff> Staff => GetStaffAsync().GetAwaiter().GetResult().AsQueryable();
        public IQueryable<Facility> Facilities => GetFacilitiesAsync().GetAwaiter().GetResult().AsQueryable();
        public IQueryable<Reservation> Reservations => GetReservationsAsync().GetAwaiter().GetResult().AsQueryable();
        public IQueryable<ServiceRequest> ServiceRequests => GetServiceRequestsAsync().GetAwaiter().GetResult().AsQueryable();
        public IQueryable<Announcement> Announcements => GetAnnouncementsAsync().GetAwaiter().GetResult().AsQueryable();
        public IQueryable<ForumPost> ForumPosts => GetForumPostsAsync().GetAwaiter().GetResult().AsQueryable();
        public IQueryable<ForumComment> ForumComments => GetForumCommentsAsync().GetAwaiter().GetResult().AsQueryable();
        public IQueryable<Reaction> Reactions => GetReactionsAsync().GetAwaiter().GetResult().AsQueryable();
        public IQueryable<EventModel> Events => GetEventsAsync().GetAwaiter().GetResult().AsQueryable();
        public IQueryable<HomeownerProfileImage> HomeownerProfileImages => GetHomeownerProfileImagesAsync().GetAwaiter().GetResult().AsQueryable();

        // Billing
        public async Task<List<Billing>> GetBillingsAsync()
        {
            var snapshot = await BillingsCollection.GetSnapshotAsync();
            return snapshot.Documents.Select(doc => doc.ConvertTo<Billing>()).ToList();
        }

        public async Task<Billing?> GetBillingByIdAsync(int id)
        {
            var doc = await BillingsCollection.Document(id.ToString()).GetSnapshotAsync();
            return doc.Exists ? doc.ConvertTo<Billing>() : null;
        }

        public async Task<List<Billing>> GetBillingsByHomeownerIdAsync(int homeownerId)
        {
            var query = BillingsCollection.WhereEqualTo("HomeownerID", homeownerId);
            var snapshot = await query.GetSnapshotAsync();
            return snapshot.Documents.Select(doc => doc.ConvertTo<Billing>()).ToList();
        }

        public async Task AddBillingAsync(Billing billing)
        {
            // Auto-generate BillingID if not set
            if (billing.BillingID == 0)
            {
                var allBillings = await GetBillingsAsync();
                billing.BillingID = allBillings.Any() ? allBillings.Max(b => b.BillingID) + 1 : 1;
            }
            await BillingsCollection.Document(billing.BillingID.ToString()).SetAsync(billing);
        }

        public async Task UpdateBillingAsync(Billing billing)
        {
            await BillingsCollection.Document(billing.BillingID.ToString()).SetAsync(billing);
        }

        public async Task DeleteBillingAsync(int id)
        {
            await BillingsCollection.Document(id.ToString()).DeleteAsync();
        }

        public IQueryable<Billing> Billings => GetBillingsAsync().GetAwaiter().GetResult().AsQueryable();

        // Documents
        public async Task<List<Document>> GetDocumentsAsync()
        {
            var snapshot = await DocumentsCollection.GetSnapshotAsync();
            return snapshot.Documents.Select(doc => doc.ConvertTo<Document>()).ToList();
        }

        public async Task<Document?> GetDocumentByIdAsync(int id)
        {
            var doc = await DocumentsCollection.Document(id.ToString()).GetSnapshotAsync();
            return doc.Exists ? doc.ConvertTo<Document>() : null;
        }

        public async Task<List<Document>> GetDocumentsByCategoryAsync(string category)
        {
            var query = DocumentsCollection.WhereEqualTo("Category", category);
            var snapshot = await query.GetSnapshotAsync();
            return snapshot.Documents.Select(doc => doc.ConvertTo<Document>()).ToList();
        }

        public async Task AddDocumentAsync(Document document)
        {
            if (document.DocumentID == 0)
            {
                var allDocs = await GetDocumentsAsync();
                document.DocumentID = allDocs.Any() ? allDocs.Max(d => d.DocumentID) + 1 : 1;
            }
            await DocumentsCollection.Document(document.DocumentID.ToString()).SetAsync(document);
        }

        public async Task UpdateDocumentAsync(Document document)
        {
            await DocumentsCollection.Document(document.DocumentID.ToString()).SetAsync(document);
        }

        public async Task DeleteDocumentAsync(int id)
        {
            await DocumentsCollection.Document(id.ToString()).DeleteAsync();
        }

        public async Task IncrementDownloadCountAsync(int documentId)
        {
            var doc = await GetDocumentByIdAsync(documentId);
            if (doc != null)
            {
                doc.DownloadCount++;
                await UpdateDocumentAsync(doc);
            }
        }

        public IQueryable<Document> Documents => GetDocumentsAsync().GetAwaiter().GetResult().AsQueryable();

        // Contacts
        public async Task<List<Contact>> GetContactsAsync()
        {
            var snapshot = await ContactsCollection.GetSnapshotAsync();
            return snapshot.Documents.Select(doc => doc.ConvertTo<Contact>()).ToList();
        }

        public async Task<Contact?> GetContactByIdAsync(int id)
        {
            var doc = await ContactsCollection.Document(id.ToString()).GetSnapshotAsync();
            return doc.Exists ? doc.ConvertTo<Contact>() : null;
        }

        public async Task<List<Contact>> GetContactsByCategoryAsync(string category)
        {
            var query = ContactsCollection.WhereEqualTo("Category", category);
            var snapshot = await query.GetSnapshotAsync();
            return snapshot.Documents.Select(doc => doc.ConvertTo<Contact>()).ToList();
        }

        public async Task AddContactAsync(Contact contact)
        {
            if (contact.ContactID == 0)
            {
                var allContacts = await GetContactsAsync();
                contact.ContactID = allContacts.Any() ? allContacts.Max(c => c.ContactID) + 1 : 1;
            }
            await ContactsCollection.Document(contact.ContactID.ToString()).SetAsync(contact);
        }

        public async Task UpdateContactAsync(Contact contact)
        {
            await ContactsCollection.Document(contact.ContactID.ToString()).SetAsync(contact);
        }

        public async Task DeleteContactAsync(int id)
        {
            await ContactsCollection.Document(id.ToString()).DeleteAsync();
        }

        public IQueryable<Contact> Contacts => GetContactsAsync().GetAwaiter().GetResult().AsQueryable();

        // Visitor Passes
        public async Task<List<VisitorPass>> GetVisitorPassesAsync()
        {
            var snapshot = await VisitorPassesCollection.GetSnapshotAsync();
            return snapshot.Documents.Select(doc => doc.ConvertTo<VisitorPass>()).ToList();
        }

        public async Task<VisitorPass?> GetVisitorPassByIdAsync(int id)
        {
            var doc = await VisitorPassesCollection.Document(id.ToString()).GetSnapshotAsync();
            return doc.Exists ? doc.ConvertTo<VisitorPass>() : null;
        }

        public async Task<List<VisitorPass>> GetVisitorPassesByHomeownerIdAsync(int homeownerId)
        {
            var query = VisitorPassesCollection.WhereEqualTo("HomeownerID", homeownerId);
            var snapshot = await query.GetSnapshotAsync();
            return snapshot.Documents.Select(doc => doc.ConvertTo<VisitorPass>()).ToList();
        }

        public async Task AddVisitorPassAsync(VisitorPass visitorPass)
        {
            if (visitorPass.VisitorPassID == 0)
            {
                var allPasses = await GetVisitorPassesAsync();
                visitorPass.VisitorPassID = allPasses.Any() ? allPasses.Max(v => v.VisitorPassID) + 1 : 1;
            }
            await VisitorPassesCollection.Document(visitorPass.VisitorPassID.ToString()).SetAsync(visitorPass);
        }

        public async Task UpdateVisitorPassAsync(VisitorPass visitorPass)
        {
            await VisitorPassesCollection.Document(visitorPass.VisitorPassID.ToString()).SetAsync(visitorPass);
        }

        public async Task DeleteVisitorPassAsync(int id)
        {
            await VisitorPassesCollection.Document(id.ToString()).DeleteAsync();
        }

        public IQueryable<VisitorPass> VisitorPasses => GetVisitorPassesAsync().GetAwaiter().GetResult().AsQueryable();

        // Vehicle Registration
        public async Task<List<VehicleRegistration>> GetVehicleRegistrationsAsync()
        {
            var snapshot = await VehicleRegistrationsCollection.GetSnapshotAsync();
            return snapshot.Documents.Select(doc => doc.ConvertTo<VehicleRegistration>()).ToList();
        }

        public async Task<VehicleRegistration?> GetVehicleByIdAsync(int id)
        {
            var doc = await VehicleRegistrationsCollection.Document(id.ToString()).GetSnapshotAsync();
            return doc.Exists ? doc.ConvertTo<VehicleRegistration>() : null;
        }

        public async Task<List<VehicleRegistration>> GetVehiclesByHomeownerIdAsync(int homeownerId)
        {
            var query = VehicleRegistrationsCollection.WhereEqualTo("HomeownerID", homeownerId);
            var snapshot = await query.GetSnapshotAsync();
            return snapshot.Documents.Select(doc => doc.ConvertTo<VehicleRegistration>()).ToList();
        }

        public async Task<VehicleRegistration?> GetVehicleByPlateNumberAsync(string plateNumber)
        {
            var query = VehicleRegistrationsCollection.WhereEqualTo("PlateNumber", plateNumber);
            var snapshot = await query.GetSnapshotAsync();
            return snapshot.Documents.FirstOrDefault()?.ConvertTo<VehicleRegistration>();
        }

        public async Task AddVehicleAsync(VehicleRegistration vehicle)
        {
            if (vehicle.VehicleID == 0)
            {
                var allVehicles = await GetVehicleRegistrationsAsync();
                vehicle.VehicleID = allVehicles.Any() ? allVehicles.Max(v => v.VehicleID) + 1 : 1;
            }
            await VehicleRegistrationsCollection.Document(vehicle.VehicleID.ToString()).SetAsync(vehicle);
        }

        public async Task UpdateVehicleAsync(VehicleRegistration vehicle)
        {
            await VehicleRegistrationsCollection.Document(vehicle.VehicleID.ToString()).SetAsync(vehicle);
        }

        public async Task DeleteVehicleAsync(int id)
        {
            await VehicleRegistrationsCollection.Document(id.ToString()).DeleteAsync();
        }

        public IQueryable<VehicleRegistration> VehicleRegistrations => GetVehicleRegistrationsAsync().GetAwaiter().GetResult().AsQueryable();

        // Gate Access Logs
        public async Task<List<GateAccessLog>> GetGateAccessLogsAsync()
        {
            var snapshot = await GateAccessLogsCollection.GetSnapshotAsync();
            return snapshot.Documents.Select(doc => doc.ConvertTo<GateAccessLog>()).ToList();
        }

        public async Task AddGateAccessLogAsync(GateAccessLog log)
        {
            if (log.LogID == 0)
            {
                var allLogs = await GetGateAccessLogsAsync();
                log.LogID = allLogs.Any() ? allLogs.Max(l => l.LogID) + 1 : 1;
            }
            await GateAccessLogsCollection.Document(log.LogID.ToString()).SetAsync(log);
        }

        public async Task<List<GateAccessLog>> GetGateAccessLogsByDateRangeAsync(DateTime startDate, DateTime endDate)
        {
            var allLogs = await GetGateAccessLogsAsync();
            return allLogs.Where(l => l.AccessTime >= startDate && l.AccessTime <= endDate).ToList();
        }

        public IQueryable<GateAccessLog> GateAccessLogs => GetGateAccessLogsAsync().GetAwaiter().GetResult().AsQueryable();

        // Complaints
        public async Task<List<Complaint>> GetComplaintsAsync()
        {
            var snapshot = await ComplaintsCollection.GetSnapshotAsync();
            return snapshot.Documents.Select(doc => doc.ConvertTo<Complaint>()).ToList();
        }

        public async Task<Complaint?> GetComplaintByIdAsync(int id)
        {
            var doc = await ComplaintsCollection.Document(id.ToString()).GetSnapshotAsync();
            return doc.Exists ? doc.ConvertTo<Complaint>() : null;
        }

        public async Task<List<Complaint>> GetComplaintsByHomeownerIdAsync(int homeownerId)
        {
            var query = ComplaintsCollection.WhereEqualTo("HomeownerID", homeownerId);
            var snapshot = await query.GetSnapshotAsync();
            return snapshot.Documents.Select(doc => doc.ConvertTo<Complaint>()).ToList();
        }

        public async Task AddComplaintAsync(Complaint complaint)
        {
            if (complaint.ComplaintID == 0)
            {
                var allComplaints = await GetComplaintsAsync();
                complaint.ComplaintID = allComplaints.Any() ? allComplaints.Max(c => c.ComplaintID) + 1 : 1;
            }
            await ComplaintsCollection.Document(complaint.ComplaintID.ToString()).SetAsync(complaint);
        }

        public async Task UpdateComplaintAsync(Complaint complaint)
        {
            await ComplaintsCollection.Document(complaint.ComplaintID.ToString()).SetAsync(complaint);
        }

        public async Task DeleteComplaintAsync(int id)
        {
            await ComplaintsCollection.Document(id.ToString()).DeleteAsync();
        }

        public IQueryable<Complaint> Complaints => GetComplaintsAsync().GetAwaiter().GetResult().AsQueryable();

        // Polls
        public async Task<List<Poll>> GetPollsAsync()
        {
            var snapshot = await PollsCollection.GetSnapshotAsync();
            var polls = snapshot.Documents.Select(doc => doc.ConvertTo<Poll>()).ToList();
            
            // Load options and votes for each poll
            foreach (var poll in polls)
            {
                poll.Options = await GetPollOptionsAsync(poll.PollID);
                poll.Votes = await GetPollVotesAsync(poll.PollID);
            }
            
            return polls;
        }

        public async Task<Poll?> GetPollByIdAsync(int id)
        {
            var doc = await PollsCollection.Document(id.ToString()).GetSnapshotAsync();
            if (!doc.Exists) return null;
            
            var poll = doc.ConvertTo<Poll>();
            poll.Options = await GetPollOptionsAsync(id);
            poll.Votes = await GetPollVotesAsync(id);
            return poll;
        }

        public async Task<List<Poll>> GetActivePollsAsync()
        {
            var now = DateTime.UtcNow;
            var allPolls = await GetPollsAsync();
            return allPolls.Where(p => p.Status == "Active" && 
                (p.StartDate == null || p.StartDate <= now) &&
                (p.EndDate == null || p.EndDate >= now)).ToList();
        }

        public async Task AddPollAsync(Poll poll)
        {
            if (poll.PollID == 0)
            {
                var allPolls = await GetPollsAsync();
                poll.PollID = allPolls.Any() ? allPolls.Max(p => p.PollID) + 1 : 1;
            }
            await PollsCollection.Document(poll.PollID.ToString()).SetAsync(poll);
            
            // Save options separately
            foreach (var option in poll.Options)
            {
                option.PollID = poll.PollID;
                await AddPollOptionAsync(option);
            }
        }

        public async Task UpdatePollAsync(Poll poll)
        {
            await PollsCollection.Document(poll.PollID.ToString()).SetAsync(poll);
        }

        public async Task DeletePollAsync(int id)
        {
            // Delete votes first
            var votes = await GetPollVotesAsync(id);
            foreach (var vote in votes)
            {
                await PollVotesCollection.Document(vote.VoteID.ToString()).DeleteAsync();
            }
            
            // Delete options
            var options = await GetPollOptionsAsync(id);
            foreach (var option in options)
            {
                await PollOptionsCollection.Document(option.OptionID.ToString()).DeleteAsync();
            }
            
            // Delete poll
            await PollsCollection.Document(id.ToString()).DeleteAsync();
        }

        public async Task AddPollVoteAsync(PollVote vote)
        {
            if (vote.VoteID == 0)
            {
                var allVotes = await GetPollVotesAsync(vote.PollID);
                vote.VoteID = allVotes.Any() ? allVotes.Max(v => v.VoteID) + 1 : 1;
            }
            await PollVotesCollection.Document(vote.VoteID.ToString()).SetAsync(vote);
            
            // Update option vote count
            var option = await GetPollOptionByIdAsync(vote.OptionID);
            if (option != null)
            {
                option.VoteCount++;
                await UpdatePollOptionAsync(option);
            }
            
            // Update poll total votes
            var poll = await GetPollByIdAsync(vote.PollID);
            if (poll != null)
            {
                poll.TotalVotes++;
                await UpdatePollAsync(poll);
            }
        }

        public async Task<bool> HasHomeownerVotedAsync(int pollId, int homeownerId)
        {
            var query = PollVotesCollection
                .WhereEqualTo("PollID", pollId)
                .WhereEqualTo("HomeownerID", homeownerId);
            var snapshot = await query.GetSnapshotAsync();
            return snapshot.Count > 0;
        }

        public IQueryable<Poll> Polls => GetPollsAsync().GetAwaiter().GetResult().AsQueryable();

        // Helper methods for Polls
        private async Task<List<PollOption>> GetPollOptionsAsync(int pollId)
        {
            var query = PollOptionsCollection.WhereEqualTo("PollID", pollId);
            var snapshot = await query.GetSnapshotAsync();
            return snapshot.Documents.Select(doc => doc.ConvertTo<PollOption>()).OrderBy(o => o.DisplayOrder).ToList();
        }

        private async Task<List<PollVote>> GetPollVotesAsync(int pollId)
        {
            var query = PollVotesCollection.WhereEqualTo("PollID", pollId);
            var snapshot = await query.GetSnapshotAsync();
            return snapshot.Documents.Select(doc => doc.ConvertTo<PollVote>()).ToList();
        }

        private async Task<PollOption?> GetPollOptionByIdAsync(int optionId)
        {
            var doc = await PollOptionsCollection.Document(optionId.ToString()).GetSnapshotAsync();
            return doc.Exists ? doc.ConvertTo<PollOption>() : null;
        }

        private async Task AddPollOptionAsync(PollOption option)
        {
            if (option.OptionID == 0)
            {
                var allOptions = await GetPollOptionsAsync(option.PollID);
                option.OptionID = allOptions.Any() ? allOptions.Max(o => o.OptionID) + 1 : 1;
            }
            await PollOptionsCollection.Document(option.OptionID.ToString()).SetAsync(option);
        }

        private async Task UpdatePollOptionAsync(PollOption option)
        {
            await PollOptionsCollection.Document(option.OptionID.ToString()).SetAsync(option);
        }

        // Helper methods for IQueryable properties
        private async Task<List<ForumComment>> GetForumCommentsAsync()
        {
            var snapshot = await ForumCommentsCollection.GetSnapshotAsync();
            return snapshot.Documents.Select(doc => doc.ConvertTo<ForumComment>()).ToList();
        }

        private async Task<List<Reaction>> GetReactionsAsync()
        {
            var snapshot = await ReactionsCollection.GetSnapshotAsync();
            return snapshot.Documents.Select(doc => doc.ConvertTo<Reaction>()).ToList();
        }

        private async Task<List<HomeownerProfileImage>> GetHomeownerProfileImagesAsync()
        {
            var snapshot = await HomeownerProfileImagesCollection.GetSnapshotAsync();
            return snapshot.Documents.Select(doc => doc.ConvertTo<HomeownerProfileImage>()).ToList();
        }

        // SaveChangesAsync - No-op for Firebase (operations are immediate)
        public Task<int> SaveChangesAsync()
        {
            return Task.FromResult(1);
        }
    }
}

