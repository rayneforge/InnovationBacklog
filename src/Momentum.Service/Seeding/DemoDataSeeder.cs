using Azure.Data.Tables;
using Azure.Storage.Blobs;
using Momentum.Library.Application.Ports;
using Momentum.Library.Domain.Auditing;
using Momentum.Library.Domain.Comments;
using Momentum.Library.Domain.Engagement;
using Momentum.Library.Domain.Identity;
using Momentum.Library.Domain.Requests;
using Momentum.Library.Domain.Reviews;
using Momentum.Library.Domain.Solutions;
using Momentum.Library.Domain.Visibility;
using Momentum.Library.Infrastructure.AzureStorage;

namespace Momentum.Service.Seeding;

/// <summary>
/// Wipes this application's storage and writes a demonstration dataset — the
/// Momentum museum-network demonstration workspace.
///
/// Destructive by design, so it only ever touches the tables and container this
/// application owns, and refuses to run against anything but local emulator
/// storage unless explicitly forced.
/// </summary>
public static class DemoDataSeeder
{
    private static readonly DateTimeOffset Now = DateTimeOffset.UtcNow;

    private static readonly UserId Dev = new("dev@localhost");
    private static readonly UserId CommunityCurator = new("owen.haddad@museum-demo.example");
    private static readonly UserId Registrar = new("noa.bekele@museum-demo.example");
    private static readonly UserId VisitorLead = new("iris.serrano@museum-demo.example");
    private static readonly UserId FacilitiesLead = new("felix.diallo@museum-demo.example");
    private static readonly UserId InterpretationLead = new("amina.solberg@museum-demo.example");
    private static readonly UserId CollectionsLead = new("jonah.sato@museum-demo.example");
    private static readonly UserId ProgrammeLead = new("leila.novak@museum-demo.example");

    public static bool IsEmulatorStorage(string connectionString) =>
        connectionString.Contains("UseDevelopmentStorage=true", StringComparison.OrdinalIgnoreCase)
        || connectionString.Contains("127.0.0.1", StringComparison.OrdinalIgnoreCase)
        || connectionString.Contains("localhost", StringComparison.OrdinalIgnoreCase)
        || connectionString.Contains("azurite", StringComparison.OrdinalIgnoreCase);

    public static async Task Reset(
        TableStorageOptions tableOptions,
        BlobStorageOptions blobOptions,
        CancellationToken cancellationToken = default)
    {
        var tables = new TableServiceClient(tableOptions.ConnectionString);
        foreach (var name in AppTableNames)
        {
            await tables.DeleteTableAsync(name, cancellationToken);
        }

        var blobs = new BlobServiceClient(blobOptions.ConnectionString);
        await blobs.GetBlobContainerClient(blobOptions.AttachmentsContainer)
            .DeleteIfExistsAsync(cancellationToken: cancellationToken);

        // Azurite needs a moment before a name can be reused.
        await Task.Delay(TimeSpan.FromSeconds(1), cancellationToken);
    }

    private static IEnumerable<string> AppTableNames => new[]
    {
        StorageTableNames.Requests,
        StorageTableNames.Solutions,
        StorageTableNames.RequestSolutions,
        StorageTableNames.SolutionUses,
        StorageTableNames.Comments,
        StorageTableNames.Decisions,
        StorageTableNames.AuditRecords,
        StorageTableNames.AgentRuns,
        StorageTableNames.ProcessedEvents,
        StorageTableNames.Outbox,
        StorageTableNames.ProjectionState,
        StorageTableNames.Votes,
        StorageTableNames.Contributions
    };

    public static async Task Seed(IServiceProvider services)
    {
        var requests = services.GetRequiredService<IRequestRepository>();
        var solutions = services.GetRequiredService<ISolutionRepository>();
        var relationships = services.GetRequiredService<IRequestSolutionRepository>();
        var uses = services.GetRequiredService<ISolutionUseRepository>();
        var comments = services.GetRequiredService<ICommentRepository>();
        var votes = services.GetRequiredService<IVoteRepository>();
        var contributions = services.GetRequiredService<IContributionRepository>();
        var decisions = services.GetRequiredService<IAcceptanceDecisionRepository>();
        var attachments = services.GetRequiredService<IAttachmentStore>();
        var audit = services.GetRequiredService<IAuditRepository>();

        // ---------- Solutions ----------

        var loanChecklist = new Solution
        {
            RepositoryReference = new RepositoryReference("", "", ""),
            Title = "Artwork Loan Checklist",
            Description = "A shared checklist for packing, insurance and lender approvals before an artwork leaves the museum.",
            Type = SolutionType.Pattern,

            Tags = new[] { "Artwork loans", "Collections" },
            DemoUrl = "https://museum-demo.example/guides/artwork-loans",
            SubmittedBy = Registrar,
            Owner = Registrar,
            Status = SolutionStatus.Published,
            UseCount = 5,
            AdoptedByProjects = new[] { "Visitor Welcome", "Collections Care", "Learning Studio", "Touring Exhibitions", "Gallery East" },
            CreatedAt = Now.AddDays(-96),
            UpdatedAt = Now.AddDays(-3),
            PublishedAt = Now.AddDays(-90)
        };

        var quietHourPack = new Solution
        {
            RepositoryReference = new RepositoryReference("", "", ""),
            Title = "Quiet Hour Welcome Pack",
            Description = "A visitor guide, sensory map and staff briefing for running a quieter museum session.",
            Type = SolutionType.Template,

            Tags = new[] { "Visitor access", "Quiet hours" },
            DemoUrl = "https://museum-demo.example/guides/quiet-hour-pack",
            SubmittedBy = VisitorLead,
            Owner = VisitorLead,
            Status = SolutionStatus.Published,
            UseCount = 8,
            AdoptedByProjects = new[] { "Visitor Welcome", "Collections Care", "Learning Studio", "Touring Exhibitions", "Conservation", "Gallery East", "Archives", "Community Programmes" },
            CreatedAt = Now.AddDays(-140),
            UpdatedAt = Now.AddDays(-8),
            PublishedAt = Now.AddDays(-136)
        };

        var energyLog = new Solution
        {
            RepositoryReference = new RepositoryReference("", "", ""),
            Title = "Energy Walkround Log",
            Description = "A repeatable closing walkround that records lighting, heating and equipment left running.",
            Type = SolutionType.Template,

            Tags = new[] { "Buildings", "Sustainability" },
            DemoUrl = "https://museum-demo.example/guides/energy-walkround",
            SubmittedBy = FacilitiesLead,
            Owner = FacilitiesLead,
            Status = SolutionStatus.Published,
            UseCount = 3,
            AdoptedByProjects = new[] { "Touring Exhibitions", "Visitor Welcome", "Conservation" },
            CreatedAt = Now.AddDays(-54),
            UpdatedAt = Now.AddDays(-2),
            PublishedAt = Now.AddDays(-48)
        };

        var labelGuide = new Solution
        {
            RepositoryReference = new RepositoryReference("", "", ""),
            Title = "Exhibition Label Guide",
            Description = "Plain-language label examples, translation notes and print specifications for exhibition teams.",
            Type = SolutionType.Template,

            Tags = new[] { "Interpretation", "Exhibitions" },
            DemoUrl = "https://museum-demo.example/guides/exhibition-labels",
            SubmittedBy = InterpretationLead,
            Owner = InterpretationLead,
            Status = SolutionStatus.Published,
            UseCount = 4,
            AdoptedByProjects = new[] { "Learning Studio", "Conservation", "Gallery East", "Visitor Welcome" },
            CreatedAt = Now.AddDays(-71),
            UpdatedAt = Now.AddDays(-11),
            PublishedAt = Now.AddDays(-66)
        };

        var conditionRecord = new Solution
        {
            RepositoryReference = new RepositoryReference("", "", ""),
            Title = "Object Condition Record",
            Description = "Photographs and a short condition checklist for tracking an object before and after a move.",
            Type = SolutionType.Pattern,

            Tags = new[] { "Collections care", "Object moves" },
            SubmittedBy = CollectionsLead,
            Owner = CollectionsLead,
            // Waiting on a reviewer, so it is visible only to reviewers and CollectionsLead.
            Status = SolutionStatus.AwaitingApproval,
            UseCount = 1,
            AdoptedByProjects = new[] { "Collections Care" },
            CreatedAt = Now.AddDays(-19),
            UpdatedAt = Now.AddDays(-6),
            PublishedAt = Now.AddDays(-16)
        };

        // Restricted: commercially sensitive, approvers and admins only.
        var lenderRegister = new Solution
        {
            RepositoryReference = new RepositoryReference("", "", ""),
            Title = "Lender Agreement Register",
            Description = "Loan dates, insurance limits and private negotiation notes held together for the collections team.",
            Type = SolutionType.Other,

            Tags = new[] { "Loans", "Agreements" },
            DemoUrl = "https://museum-demo.example/guides/lender-register",
            SubmittedBy = ProgrammeLead,
            Owner = ProgrammeLead,
            Visibility = ItemVisibility.Approvers,
            Status = SolutionStatus.Published,
            UseCount = 1,
            AdoptedByProjects = new[] { "Archives" },
            CreatedAt = Now.AddDays(-27),
            UpdatedAt = Now.AddDays(-4),
            PublishedAt = Now.AddDays(-25)
        };

        // Hidden: superseded, kept for the record. Administrators only.
        var stockLedger = new Solution
        {
            RepositoryReference = new RepositoryReference("", "", ""),
            Title = "Printed Stock Ledger",
            Description = "The retired paper inventory retained for historical reference after the collections move.",
            Type = SolutionType.Other,

            Tags = new[] { "Archive", "Retired" },
            SubmittedBy = FacilitiesLead,
            Owner = FacilitiesLead,
            Visibility = ItemVisibility.Hidden,
            Status = SolutionStatus.Retired,
            CreatedAt = Now.AddDays(-210),
            UpdatedAt = Now.AddDays(-30),
            PublishedAt = Now.AddDays(-205)
        };

        var allSolutions = new[]
        {
            loanChecklist, quietHourPack, energyLog, labelGuide,
            conditionRecord, lenderRegister, stockLedger
        };
        foreach (var solution in allSolutions) await solutions.Save(solution);

        // ---------- Ideas ----------

        var arrivalQueues = new Request
        {
            Type = RequestType.Backlog,
            Status = RequestStatus.Accepted,
            SubmittedBy = CommunityCurator,
            Title = "Stop queues blocking the gallery entrance",
            Tags = new[] { "Visitor arrival", "School visits" },
            Description = "School groups and individual visitors arrive through the same narrow doorway. Try separate meeting points and timed group arrivals.",
            CreatedAt = Now.AddDays(-34),
            UpdatedAt = Now.AddDays(-2)
        };

        var quietVisits = new Request
        {
            Type = RequestType.Backlog,
            Status = RequestStatus.Accepted,
            SubmittedBy = VisitorLead,
            Title = "Make quiet museum visits easier to plan",
            Tags = new[] { "Visitor access", "Family visits" },
            Description = "Families need to know the quieter route, available seating and what to expect before they arrive.",
            CanonicalSolutionId = quietHourPack.Id,
            CreatedAt = Now.AddDays(-150),
            UpdatedAt = Now.AddDays(-8)
        };

        var energyWaste = new Request
        {
            Type = RequestType.Backlog,
            Status = RequestStatus.Accepted,
            SubmittedBy = FacilitiesLead,
            Title = "Reduce energy waste after closing",
            Tags = new[] { "Sustainability", "Buildings" },
            Description = "Empty galleries are sometimes lit and heated overnight. A short handover between front of house and facilities could prevent it.",
            CreatedAt = Now.AddDays(-60),
            UpdatedAt = Now.AddDays(-2)
        };

        var loanCoordination = new Request
        {
            Type = RequestType.Backlog,
            Status = RequestStatus.Accepted,
            SubmittedBy = Registrar,
            Title = "Make artwork loans easier to coordinate",
            Tags = new[] { "Loans", "Collections" },
            Description = "Packing, insurance and courier checks are being repeated in separate documents. A shared checklist would make responsibilities clear.",
            CanonicalSolutionId = loanChecklist.Id,
            CreatedAt = Now.AddDays(-100),
            UpdatedAt = Now.AddDays(-3)
        };

        var plainLabels = new Request
        {
            Type = RequestType.Backlog,
            Status = RequestStatus.Accepted,
            SubmittedBy = InterpretationLead,
            Title = "Write exhibition labels people can understand",
            Tags = new[] { "Interpretation", "Access" },
            Description = "Long labels and unexplained specialist terms leave visitors behind. Agree a plain-language format and test it with real readers.",
            CreatedAt = Now.AddDays(-80),
            UpdatedAt = Now.AddDays(-11)
        };

        var handlingAdvice = new Request
        {
            Type = RequestType.Backlog,
            Status = RequestStatus.AwaitingApproval,
            SubmittedBy = CollectionsLead,
            Title = "Find the latest object handling instructions",
            Tags = new[] { "Collections care", "Handling" },
            Description = "Handling advice is split between filing cabinets and old emails. Staff need one trusted place to find the current instruction.",
            CreatedAt = Now.AddDays(-5),
            UpdatedAt = Now.AddDays(-5)
        };

        var welcomingRoute = new Request
        {
            Type = RequestType.Backlog,
            Status = RequestStatus.AwaitingApproval,
            SubmittedBy = ProgrammeLead,
            Title = "Offer a welcoming route for anxious visitors",
            Tags = new[] { "Visitor welcome", "Inclusion" },
            Description = "Some visitors need a quieter arrival, clear signs and a place to pause. Invite families to help us plan the route.",
            CreatedAt = Now.AddDays(-2),
            UpdatedAt = Now.AddDays(-2)
        };

        var villageVisits = new Request
        {
            Type = RequestType.Backlog,
            Status = RequestStatus.Created,
            SubmittedBy = CommunityCurator,
            Title = "Bring museum visits to isolated villages",
            Tags = new[] { "Community", "Outreach" },
            Description = "Community groups without transport rarely reach the museum. Trial a small travelling collection with local hosts.",
            CreatedAt = Now.AddDays(-9),
            UpdatedAt = Now.AddDays(-9)
        };

        var eveningProgramme = new Request
        {
            Type = RequestType.Backlog,
            Status = RequestStatus.Created,
            SubmittedBy = Registrar,
            Title = "Let visitors shape the next evening opening",
            Tags = new[] { "Events", "Audience research" },
            Description = "Ask local residents which talks, music and activities would make an evening visit worth the journey.",
            CreatedAt = Now.AddDays(-13),
            UpdatedAt = Now.AddDays(-13)
        };

        var sharedStorage = new Request
        {
            Type = RequestType.Backlog,
            Status = RequestStatus.Rejected,
            SubmittedBy = CollectionsLead,
            Title = "Store every collection in one shared room",
            Tags = new[] { "Storage", "Conservation" },
            Description = "Combining all storage would simplify retrieval but put delicate objects and everyday handling in the same space.",
            CreatedAt = Now.AddDays(-45),
            UpdatedAt = Now.AddDays(-40)
        };

        // Restricted: names a partner, so approvers and admins only.
        var touringLoan = new Request
        {
            Type = RequestType.Backlog,
            Status = RequestStatus.AwaitingApproval,
            SubmittedBy = ProgrammeLead,
            Visibility = ItemVisibility.Approvers,
            Title = "Agree the Seabrook loan terms before the tour",
            Tags = new[] { "Touring exhibitions", "Loan agreements" },
            Description = "The touring exhibition needs insurance, transport and display conditions agreed with the lender. Details remain restricted during negotiations.",
            CreatedAt = Now.AddDays(-7),
            UpdatedAt = Now.AddDays(-7)
        };

        var allRequests = new[]
        {
            arrivalQueues, quietVisits, energyWaste, loanCoordination, plainLabels, handlingAdvice,
            welcomingRoute, villageVisits, eveningProgramme, sharedStorage, touringLoan
        };
        foreach (var request in allRequests) await requests.Save(request);

        // ---------- Links between ideas and solutions ----------

        await Link(relationships, loanCoordination, loanChecklist, RequestSolutionRelationship.Existing, Registrar, Now.AddDays(-88));
        await Link(relationships, arrivalQueues, loanChecklist, RequestSolutionRelationship.Relevant, CommunityCurator, Now.AddDays(-30), ApprovalState.Pending);
        await Link(relationships, quietVisits, quietHourPack, RequestSolutionRelationship.Existing, VisitorLead, Now.AddDays(-134));
        await Link(relationships, energyWaste, energyLog, RequestSolutionRelationship.Existing, FacilitiesLead, Now.AddDays(-46));
        await Link(relationships, energyWaste, stockLedger, RequestSolutionRelationship.Proposed, FacilitiesLead, Now.AddDays(-52));
        await Link(relationships, plainLabels, labelGuide, RequestSolutionRelationship.Existing, InterpretationLead, Now.AddDays(-64));
        await Link(relationships, handlingAdvice, conditionRecord, RequestSolutionRelationship.Proposed, CollectionsLead, Now.AddDays(-4), ApprovalState.Pending);
        await Link(relationships, touringLoan, lenderRegister, RequestSolutionRelationship.Relevant, ProgrammeLead, Now.AddDays(-6));

        // ---------- Adoption ----------

        await Use(uses, loanChecklist, Registrar, "Visitor Welcome", "Visitor Welcome", SolutionUseStatus.Using, Now.AddDays(-70), completed: Now.AddDays(-40));
        await Use(uses, loanChecklist, CommunityCurator, "Collections Care", "Collections Care", SolutionUseStatus.Using, Now.AddDays(-52), completed: Now.AddDays(-20));
        await Use(uses, loanChecklist, InterpretationLead, "Learning Studio", "Learning Studio", SolutionUseStatus.Implementing, Now.AddDays(-14));
        await Use(uses, loanChecklist, FacilitiesLead, "Touring Exhibitions", "Touring Exhibitions", SolutionUseStatus.Exploring, Now.AddDays(-3));

        await Use(uses, quietHourPack, VisitorLead, "Visitor Welcome", "Visitor Welcome", SolutionUseStatus.Using, Now.AddDays(-120), completed: Now.AddDays(-100));
        await Use(uses, quietHourPack, CommunityCurator, "Collections Care", "Collections Care", SolutionUseStatus.Using, Now.AddDays(-110), completed: Now.AddDays(-95));
        await Use(uses, quietHourPack, Registrar, "Learning Studio", "Learning Studio", SolutionUseStatus.Using, Now.AddDays(-88), completed: Now.AddDays(-70));
        await Use(uses, quietHourPack, FacilitiesLead, "Touring Exhibitions", "Touring Exhibitions", SolutionUseStatus.Using, Now.AddDays(-60), completed: Now.AddDays(-44));
        await Use(uses, quietHourPack, CollectionsLead, "Conservation", "Conservation", SolutionUseStatus.Implementing, Now.AddDays(-12));
        await Use(uses, quietHourPack, ProgrammeLead, "Gallery East", "Gallery East", SolutionUseStatus.Exploring, Now.AddDays(-5));

        await Use(uses, energyLog, FacilitiesLead, "Touring Exhibitions", "Touring Exhibitions", SolutionUseStatus.Using, Now.AddDays(-40), completed: Now.AddDays(-18));
        await Use(uses, energyLog, Registrar, "Visitor Welcome", "Visitor Welcome", SolutionUseStatus.Implementing, Now.AddDays(-9));

        await Use(uses, labelGuide, InterpretationLead, "Learning Studio", "Learning Studio", SolutionUseStatus.Using, Now.AddDays(-58), completed: Now.AddDays(-30));
        await Use(uses, labelGuide, CollectionsLead, "Conservation", "Conservation", SolutionUseStatus.Implementing, Now.AddDays(-16));
        await Use(uses, labelGuide, ProgrammeLead, "Gallery East", "Gallery East", SolutionUseStatus.Exploring, Now.AddDays(-6));

        await Use(uses, conditionRecord, CommunityCurator, "Collections Care", "Collections Care", SolutionUseStatus.Exploring, Now.AddDays(-6));
        await Use(uses, lenderRegister, ProgrammeLead, "Archives", "Archives", SolutionUseStatus.Implementing, Now.AddDays(-11));

        await Use(uses, quietHourPack, InterpretationLead, "Archives", "Archives", SolutionUseStatus.Using, Now.AddDays(-21));
        await Use(uses, quietHourPack, Dev, "Community Programmes", "Community Programmes", SolutionUseStatus.Exploring, Now.AddDays(-2));
        await Use(uses, loanChecklist, CollectionsLead, "Gallery East", "Gallery East", SolutionUseStatus.Using, Now.AddDays(-17));
        await Use(uses, energyLog, ProgrammeLead, "Conservation", "Conservation", SolutionUseStatus.Exploring, Now.AddDays(-4));
        await Use(uses, labelGuide, VisitorLead, "Visitor Welcome", "Visitor Welcome", SolutionUseStatus.Using, Now.AddDays(-13));

        // ---------- Upvotes ----------

        await Upvote(votes, HubItemReference.ForRequest(arrivalQueues.Id), new[] { CommunityCurator, Registrar, VisitorLead, FacilitiesLead, InterpretationLead, CollectionsLead, ProgrammeLead, Dev }, Now.AddDays(-20));
        await Upvote(votes, HubItemReference.ForRequest(energyWaste.Id), new[] { FacilitiesLead, Registrar, VisitorLead, InterpretationLead, CollectionsLead }, Now.AddDays(-15));
        await Upvote(votes, HubItemReference.ForRequest(plainLabels.Id), new[] { InterpretationLead, ProgrammeLead, VisitorLead, CollectionsLead }, Now.AddDays(-40));
        await Upvote(votes, HubItemReference.ForRequest(handlingAdvice.Id), new[] { CollectionsLead, CommunityCurator, Registrar, FacilitiesLead, ProgrammeLead }, Now.AddDays(-4));
        await Upvote(votes, HubItemReference.ForRequest(welcomingRoute.Id), new[] { ProgrammeLead, InterpretationLead, VisitorLead }, Now.AddDays(-2));
        await Upvote(votes, HubItemReference.ForRequest(villageVisits.Id), new[] { CommunityCurator, Registrar }, Now.AddDays(-8));
        await Upvote(votes, HubItemReference.ForRequest(eveningProgramme.Id), new[] { Registrar }, Now.AddDays(-12));
        await Upvote(votes, HubItemReference.ForRequest(quietVisits.Id), new[] { VisitorLead, CommunityCurator, Registrar, FacilitiesLead, CollectionsLead, InterpretationLead }, Now.AddDays(-100));
        await Upvote(votes, HubItemReference.ForRequest(loanCoordination.Id), new[] { Registrar, CommunityCurator, FacilitiesLead }, Now.AddDays(-80));
        await Upvote(votes, HubItemReference.ForSolution(quietHourPack.Id), new[] { VisitorLead, CommunityCurator, FacilitiesLead, CollectionsLead }, Now.AddDays(-50));
        await Upvote(votes, HubItemReference.ForSolution(loanChecklist.Id), new[] { Registrar, InterpretationLead, FacilitiesLead }, Now.AddDays(-35));

        // ---------- Conversation ----------
        var report = await attachments.Save("arrival-observations.csv", "text/csv",
            System.Text.Encoding.UTF8.GetBytes("session,group_count,wait_minutes\nTuesday morning,7,18\nThursday afternoon,4,9\n"));
        var labelWorkshop = await attachments.Save("label-workshop-plan.md", "text/markdown",
            System.Text.Encoding.UTF8.GetBytes("# Exhibition label workshop\n\nTest one label with visitors, collect unfamiliar words, and agree a print proof.\n"));
        await Comment(comments, arrivalQueues, CommunityCurator, CommentAudience.Authenticated, "Two school groups met at the same door. The observation sheet records where the queue formed.", Now.AddDays(-17), new[] { report });
        await Comment(comments, arrivalQueues, Registrar, CommentAudience.Authenticated, "Collections Care can arrange a separate meeting point for the next school visit.", Now.AddDays(-5));
        await Comment(comments, arrivalQueues, FacilitiesLead, CommentAudience.Authenticated, "The loan checklist has a useful handover section we could adapt for group arrivals.", Now.AddDays(-16));
        await Comment(comments, plainLabels, InterpretationLead, CommentAudience.Authenticated, "Workshop plan attached. Start with one label and ask visitors to explain it back to us.", Now.AddDays(-17), new[] { labelWorkshop });
        await Comment(comments, plainLabels, ProgrammeLead, CommentAudience.Authenticated, "Gallery East would like to try the first draft with our weekend volunteers.", Now.AddDays(-14));
        await Comment(comments, energyWaste, FacilitiesLead, CommentAudience.Authenticated, "The walkround log helped the closing team find two rooms with the lights still on.", Now.AddDays(-12));
        await Comment(comments, energyWaste, VisitorLead, CommentAudience.Authenticated, "Can the final person on duty sign the sheet before the building is locked?", Now.AddDays(-16));
        await Comment(comments, handlingAdvice, CommunityCurator, CommentAudience.Authenticated, "I can help check which handling instructions are still current.", Now.AddDays(-2));
        await Comment(comments, welcomingRoute, ProgrammeLead, CommentAudience.Authenticated, "Let us invite the family advisory group before choosing the route.", Now.AddDays(-1));
        await Comment(comments, sharedStorage, Dev, CommentAudience.Authenticated, "Separate environmental conditions are essential for delicate objects. We should improve retrieval without combining all storage.", Now.AddDays(-17));
        await CommentOnSolution(comments, loanChecklist, CommunityCurator, CommentAudience.Authenticated, "The courier and insurance checks are now in one place.", Now.AddDays(-7));
        await CommentOnSolution(comments, quietHourPack, CollectionsLead, CommentAudience.Authenticated, "The quieter route is ready for the next family session.", Now.AddDays(-5));
        await CommentOnSolution(comments, energyLog, Registrar, CommentAudience.Authenticated, "Our closing team added the equipment cupboard to the walkround.", Now.AddDays(-7));
        await CommentOnSolution(comments, labelGuide, ProgrammeLead, CommentAudience.Authenticated, "We tried the shorter labels with visitors and kept their wording.", Now.AddDays(-3));

        // ---------- Participation requests awaiting a decision ----------

        await contributions.Save(new Contribution
        {
            Target = HubItemReference.ForRequest(handlingAdvice.Id),
            RequestedBy = CommunityCurator,
            Message = "I can check the object handling instructions with the conservation team.",
            Status = ContributionStatus.Accepted,
            CreatedAt = Now.AddDays(-3),
            UpdatedAt = Now.AddDays(-3)
        });
        await contributions.Save(new Contribution
        {
            Target = HubItemReference.ForSolution(labelGuide.Id),
            RequestedBy = ProgrammeLead,
            Message = "I would like to test the label guide with our gallery volunteers.",
            Status = ContributionStatus.Accepted,
            CreatedAt = Now.AddDays(-2),
            UpdatedAt = Now.AddDays(-2)
        });
        await contributions.Save(new Contribution
        {
            Target = HubItemReference.ForRequest(welcomingRoute.Id),
            RequestedBy = InterpretationLead,
            Message = "I can invite the family advisory group and help with the quiet-route trial.",
            Status = ContributionStatus.Accepted,
            CreatedAt = Now.AddDays(-1),
            UpdatedAt = Now.AddDays(-1)
        });
        await contributions.Save(new Contribution
        {
            Target = HubItemReference.ForSolution(loanChecklist.Id),
            RequestedBy = FacilitiesLead,
            Message = "Our touring exhibition team can pilot the loan checklist.",
            Status = ContributionStatus.Accepted,
            DecidedBy = Dev,
            Rationale = "Welcome — coordinate the loan trial with the collections owner.",
            CreatedAt = Now.AddDays(-9),
            UpdatedAt = Now.AddDays(-7),
            DecidedAt = Now.AddDays(-7)
        });

        // ---------- Approval decisions ----------

        await decisions.Save(new AcceptanceDecision
        {
            RequestId = arrivalQueues.Id,
            ApproverId = Dev,
            Decision = AcceptanceDecisionType.Accept,
            Rationale = "Approve a supervised trial with timed arrivals and visitor feedback.",
            DecidedAt = Now.AddDays(-2)
        });
        await decisions.Save(new AcceptanceDecision
        {
            RequestId = sharedStorage.Id,
            ApproverId = Dev,
            Decision = AcceptanceDecisionType.Reject,
            Rationale = "Different collections require different environmental conditions; a shared storage room is unsuitable.",
            DecidedAt = Now.AddDays(-40)
        });
        await decisions.Save(new AcceptanceDecision
        {
            RequestId = energyWaste.Id,
            ApproverId = Dev,
            Decision = AcceptanceDecisionType.Accept,
            Rationale = "A short closing walkround is inexpensive to trial and has a clear owner.",
            DecidedAt = Now.AddDays(-50)
        });

        // ---------- Activity feed ----------

        await SeedAudit(audit, allRequests, allSolutions);
    }

    private static async Task Link(
        IRequestSolutionRepository relationships,
        Request request,
        Solution solution,
        RequestSolutionRelationship relationship,
        UserId actor,
        DateTimeOffset at,
        ApprovalState approval = ApprovalState.Approved) =>
        await relationships.Save(new RequestSolution
        {
            RequestId = request.Id,
            SolutionId = solution.Id,
            Relationship = relationship,
            Approval = approval,
            AddedBy = actor,
            AddedAt = at,
            DecidedBy = approval == ApprovalState.Approved ? Dev : null,
            DecidedAt = approval == ApprovalState.Approved ? at : null
        });

    private static async Task Use(
        ISolutionUseRepository uses,
        Solution solution,
        UserId actor,
        string project,
        string? team,
        SolutionUseStatus status,
        DateTimeOffset started,
        DateTimeOffset? completed = null) =>
        await uses.Save(new SolutionUse
        {
            SolutionId = solution.Id,
            StartedBy = actor,
            ProjectName = project,
            Team = team,
            Status = status,
            StartedAt = started,
            UpdatedAt = completed ?? started,
            CompletedAt = completed
        });

    private static async Task Upvote(
        IVoteRepository votes,
        HubItemReference target,
        IEnumerable<UserId> voters,
        DateTimeOffset from)
    {
        var offset = 0;
        foreach (var voter in voters)
        {
            await votes.Save(new Vote
            {
                Target = target,
                UserId = voter,
                CreatedAt = from.AddHours(offset * 7)
            });
            offset++;
        }
    }

    private static Task Comment(
        ICommentRepository comments,
        Request request,
        UserId author,
        CommentAudience audience,
        string body,
        DateTimeOffset at,
        IReadOnlyList<CommentAttachment>? attachments = null) =>
        comments.Add(new Comment
        {
            SubjectId = request.Id,
            SubjectType = HubItemType.Request,
            AuthorId = author,
            Audience = audience,
            Body = body,
            Attachments = attachments ?? Array.Empty<CommentAttachment>(),
            CreatedAt = at
        });

    private static Task CommentOnSolution(
        ICommentRepository comments,
        Solution solution,
        UserId author,
        CommentAudience audience,
        string body,
        DateTimeOffset at) =>
        comments.Add(new Comment
        {
            SubjectId = solution.Id,
            SubjectType = HubItemType.Solution,
            AuthorId = author,
            Audience = audience,
            Body = body,
            CreatedAt = at
        });

    /// <summary>
    /// Audit records are what the activity rail and "what's happening" pane read,
    /// so the demo needs a believable history rather than a burst of writes dated now.
    /// </summary>
    private static async Task SeedAudit(
        IAuditRepository audit,
        IReadOnlyList<Request> requests,
        IReadOnlyList<Solution> solutions)
    {
        foreach (var request in requests)
        {
            await audit.Append(Record(
                "request.created", "request", request.Id, request.SubmittedBy.Value,
                "Shared an idea.", request.CreatedAt));
        }

        foreach (var solution in solutions)
        {
            await audit.Append(Record(
                "solution.created", "solution", solution.Id, (solution.Owner ?? solution.SubmittedBy).Value,
                "Shared a solution.", solution.CreatedAt));
        }

        var flaky = requests[0];
        var onboarding = requests[1];
        var cost = requests[2];
        var review = requests[3];
        var design = requests[4];
        var search = requests[5];
        var accessibility = requests[6];

        var copilot = solutions[0];
        var kit = solutions[1];
        var lens = solutions[2];
        var tokens = solutions[3];

        await audit.Append(Record("request.accepted", "decision", flaky.Id, Dev.Value, "Accepted the idea.", Now.AddDays(-2)));
        await audit.Append(Record("request.accepted", "decision", cost.Id, Dev.Value, "Accepted the idea.", Now.AddDays(-50)));
        await audit.Append(Record("request.accepted", "decision", review.Id, Dev.Value, "Accepted the idea.", Now.AddDays(-92)));
        await audit.Append(Record("request.rejected", "decision", requests[9].Id, Dev.Value, "Rejected the idea.", Now.AddDays(-40)));

        await audit.Append(Record("request.solutionLinked", "requestSolution", review.Id, Registrar.Value, "Linked a solution.", Now.AddDays(-88)));
        await audit.Append(Record("request.solutionLinked", "requestSolution", cost.Id, FacilitiesLead.Value, "Linked a solution.", Now.AddDays(-46)));
        await audit.Append(Record("request.solutionLinked", "requestSolution", design.Id, InterpretationLead.Value, "Linked a solution.", Now.AddDays(-64)));
        await audit.Append(Record("request.solutionLinked", "requestSolution", search.Id, CollectionsLead.Value, "Linked a solution.", Now.AddDays(-4)));

        await audit.Append(Record("request.canonicalSelected", "request", onboarding.Id, Dev.Value, "Chose the answer.", Now.AddDays(-130)));
        await audit.Append(Record("request.canonicalSelected", "request", review.Id, Dev.Value, "Chose the answer.", Now.AddDays(-86)));

        await audit.Append(Record("solutionUse.started", "solutionUse", copilot.Id, InterpretationLead.Value, "Started using a solution.", Now.AddDays(-14)));
        await audit.Append(Record("solutionUse.started", "solutionUse", copilot.Id, FacilitiesLead.Value, "Started using a solution.", Now.AddDays(-3)));
        await audit.Append(Record("solutionUse.started", "solutionUse", kit.Id, CollectionsLead.Value, "Started using a solution.", Now.AddDays(-12)));
        await audit.Append(Record("solutionUse.started", "solutionUse", kit.Id, ProgrammeLead.Value, "Started using a solution.", Now.AddDays(-5)));
        await audit.Append(Record("solutionUse.started", "solutionUse", tokens.Id, ProgrammeLead.Value, "Started using a solution.", Now.AddDays(-6)));
        await audit.Append(Record("solutionUse.completed", "solutionUse", lens.Id, FacilitiesLead.Value, "Finished a rollout.", Now.AddDays(-18)));
        await audit.Append(Record("solutionUse.completed", "solutionUse", copilot.Id, CommunityCurator.Value, "Finished a rollout.", Now.AddDays(-20)));

        // Resource types match what the real handlers write, so the demo feed is
        // shaped exactly like a feed produced by ordinary use.
        await audit.Append(Record("vote.added", "vote", flaky.Id, ProgrammeLead.Value, "Upvoted an idea.", Now.AddDays(-1)));
        await audit.Append(Record("vote.added", "vote", search.Id, CommunityCurator.Value, "Upvoted an idea.", Now.AddDays(-4)));
        await audit.Append(Record("vote.added", "vote", accessibility.Id, InterpretationLead.Value, "Upvoted an idea.", Now.AddDays(-2)));

        // comment.added rows exist for the hub feed; the item timeline hides them
        // because the comments themselves are already shown there.
        await audit.Append(Record("comment.added", "comment", flaky.Id, Registrar.Value, "Added a comment.", Now.AddDays(-31)));
        await audit.Append(Record("comment.added", "comment", copilot.Id, CommunityCurator.Value, "Added a comment.", Now.AddDays(-18)));
        await audit.Append(Record("comment.added", "comment", search.Id, CommunityCurator.Value, "Added a comment.", Now.AddDays(-4)));

        await audit.Append(Record("contribution.created", "contribution", search.Id, CommunityCurator.Value, "Asked to help.", Now.AddDays(-3)));
        await audit.Append(Record("contribution.created", "contribution", tokens.Id, ProgrammeLead.Value, "Asked to help.", Now.AddDays(-2)));
        await audit.Append(Record("contribution.created", "contribution", accessibility.Id, InterpretationLead.Value, "Asked to help.", Now.AddDays(-1)));
        await audit.Append(Record("contribution.accepted", "contribution", copilot.Id, Dev.Value, "Accepted a participation request.", Now.AddDays(-7)));

        await audit.Append(Record("solution.published", "solution", kit.Id, VisitorLead.Value, "Published a solution.", Now.AddDays(-136)));
        await audit.Append(Record("solution.published", "solution", copilot.Id, Registrar.Value, "Published a solution.", Now.AddDays(-90)));
        await audit.Append(Record("solution.published", "solution", lens.Id, FacilitiesLead.Value, "Published a solution.", Now.AddDays(-48)));
    }

    private static AuditRecord Record(
        string action,
        string resourceType,
        string subjectId,
        string actorId,
        string summary,
        DateTimeOffset at) => new()
    {
        Action = action,
        ResourceType = resourceType,
        ResourceId = subjectId,
        SubjectId = subjectId,
        ActorType = AuditActorType.User,
        ActorId = actorId,
        Summary = summary,
        OccurredAt = at
    };
}
