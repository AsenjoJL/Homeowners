using HOMEOWNER.Data;
using HOMEOWNER.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HOMEOWNER.Controllers
{
    [Authorize(Roles = "Homeowner,Admin,Staff")]
    public class ForumController : BaseController
    {
    private readonly IWebHostEnvironment _hostingEnvironment;

    public ForumController(IDataService data, IWebHostEnvironment hostingEnvironment) : base(data)
    {
        _hostingEnvironment = hostingEnvironment;
    }

    // Original Index action (for standalone forum page)
    public IActionResult Index()
    {
        // Get forum posts from Firebase
        var posts = _data.ForumPosts
            .OrderByDescending(p => p.CreatedAt)
            .ToList();

        return View(posts);
    }

    // New action for embedded forum content
    public IActionResult Embedded()
    {
        var posts = _data.ForumPosts
            .OrderByDescending(p => p.CreatedAt)
            .ToList();
            
        return PartialView("_ForumPartial", posts);
    }

    [HttpPost]
    public async Task<IActionResult> CreatePost([FromForm] string title, [FromForm] string content,
    IFormFile mediaFile, IFormFile musicFile)
    {
        // 1. Validate required fields
        if (string.IsNullOrWhiteSpace(content))
        {
            return BadRequest("Post content is required.");
        }

        var homeownerId = HttpContext.Session.GetInt32("HomeownerID");
        if (homeownerId == null) return Unauthorized();

        // 2. Create post with validated content
        var post = new ForumPost
        {
            Title = title ?? "", // Handle null title (column is NOT NULL in DB)
            Content = content,   // Guaranteed not null due to validation
            HomeownerID = homeownerId.Value,
            CreatedAt = DateTime.Now
        };

        // 3. Handle media file (optional)
        if (mediaFile != null && mediaFile.Length > 0)
        {
            const int maxMediaSize = 20 * 1024 * 1024; // 20MB
            if (mediaFile.Length > maxMediaSize)
            {
                return BadRequest("Media file exceeds 20MB limit.");
            }

            post.MediaUrl = await UploadFile(mediaFile, "media");
            post.MediaType = mediaFile.ContentType.StartsWith("image") ? "image" :
                             mediaFile.ContentType.StartsWith("video") ? "video" : null;
        }

        // 4. Handle music file (optional)
        if (musicFile != null && musicFile.Length > 0)
        {
            const int maxMusicSize = 10 * 1024 * 1024; // 10MB
            if (musicFile.Length > maxMusicSize)
            {
                return BadRequest("Music file exceeds 10MB limit.");
            }

            post.MusicUrl = await UploadFile(musicFile, "music");
            post.MusicTitle = Path.GetFileNameWithoutExtension(musicFile.FileName);
        }

        // 5. Save to Firebase
        try
        {
            await _data.AddForumPostAsync(post);
            return Ok(new { PostId = post.ForumPostID });
        }
        catch (Exception ex)
        {
            return StatusCode(500, "Error creating post: " + ex.Message);
        }
    }


    [HttpPost]
    public async Task<IActionResult> AddComment(int postId, string commentText, IFormFile mediaFile)
    {
        var homeownerId = HttpContext.Session.GetInt32("HomeownerID");
        if (homeownerId == null) return Unauthorized();

        var comment = new ForumComment
        {
            ForumPostID = postId,
            HomeownerID = homeownerId.Value,
            CommentText = commentText,
            CreatedAt = DateTime.Now
        };

        if (mediaFile != null && mediaFile.Length > 0)
        {
            var mediaPath = await UploadFile(mediaFile, "comments");
            comment.MediaUrl = mediaPath;
        }

        await _data.AddForumCommentAsync(comment);
        return Ok();
    }

    [HttpPost]
    public async Task<IActionResult> AddReaction(int postId, string reactionType)
    {
        var homeownerId = HttpContext.Session.GetInt32("HomeownerID");
        if (homeownerId == null) return Unauthorized();

        // Check if the user already reacted to this post
        var existingReaction = _data.Reactions
            .FirstOrDefault(r => r.ForumPostID == postId && r.HomeownerID == homeownerId.Value);

        if (existingReaction == null)
        {
            // Create new reaction
            var reaction = new Reaction
            {
                ForumPostID = postId,
                HomeownerID = homeownerId.Value,
                ReactionType = reactionType,
                CreatedAt = DateTime.Now
            };
            await _data.AddReactionAsync(reaction);
        }

        return Ok();
    }

    // ... (keep all other existing methods unchanged)

    private async Task<string> UploadFile(IFormFile file, string subfolder)
    {
        var uploadsFolder = Path.Combine(_hostingEnvironment.WebRootPath, "uploads", subfolder);
        if (!Directory.Exists(uploadsFolder))
        {
            Directory.CreateDirectory(uploadsFolder);
        }

        var uniqueFileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
        var filePath = Path.Combine(uploadsFolder, uniqueFileName);

        using (var fileStream = new FileStream(filePath, FileMode.Create))
        {
            await file.CopyToAsync(fileStream);
        }

        return $"/uploads/{subfolder}/{uniqueFileName}".Replace("\\", "/");
    }

    [HttpPost]
    public async Task<IActionResult> UpdateBackground(IFormFile backgroundImage, string customCSS)
    {
        // TODO: Implement CommunitySettings in Firebase
        return RedirectToAction("Index");
    }

    [HttpPost]
    public async Task<IActionResult> SetFeaturedMusic(IFormFile musicFile)
    {
        // TODO: Implement CommunitySettings in Firebase
        return Json(new { success = true, musicUrl = "" });
    }

    }
}