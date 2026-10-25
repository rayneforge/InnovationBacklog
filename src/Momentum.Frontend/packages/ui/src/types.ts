import type {
  ActivityResponseItem,
  AttachmentResponse,
  RequestResponse,
  RequestSummaryEntry,
  SolutionResponse,
  SolutionSummaryEntry,
  CommentResponse,
  SearchResponseItem,
  SolutionUseResponse,
  VoteSummaryResponse,
} from "@momentum/sdk";

export type View =
  | "home"
  | "requests"
  | "solutions"
  | "search"
  | "approvals"
  | "dashboard";

export type ContributionKind = "request" | "solution";

export type Request = RequestResponse;

export type Solution = SolutionResponse;

/** Optional attachment URL supplied by the host; otherwise use /api/attachments/{id}. */
export type Attachment = AttachmentResponse & { url?: string | null };

export type VoteSummary = VoteSummaryResponse;

export type RequestSolution = {
  requestId: string;
  solutionId: string;
  relationship: "Proposed" | "Relevant" | "Existing";
  addedBy: string;
  addedAt: string;
};

/** Optional display name resolved by the host; otherwise callers use the stored identity key. */
export type SolutionUse = SolutionUseResponse & { startedByName?: string | null };

/** Same as the wire, except its attachments may know where they live. */
export type Comment = Omit<CommentResponse, "attachments"> & {
  attachments: Attachment[];
};

export type SearchItem = SearchResponseItem;

export type SearchResult = { items: SearchItem[]; totalCount: number };

// `submittedBy` and `subtype` come from SearchItem; the rest are derived
// client-side from the workspace summaries.
export type DiscoveryItem = SearchItem & {
  kind: "Need" | "Solution";
  source: "request" | "solution";
  voteCount?: number;
  votes30d?: number;
  adoptionCount?: number;
  contributors?: number;
  linkedSolutions?: number;
  teams?: number;
  linkedNeeds?: number;
  derivedStatus?: string;
};

/** Engagement counts keyed by item id, from GET /api/requests|solutions/summary. */
export type RequestSummary = Record<string, RequestSummaryEntry>;

export type SolutionSummary = Record<string, SolutionSummaryEntry>;

/** Who may see an item. Only administrators can change it. */
export type Visibility = "Everyone" | "Approvers" | "Hidden";

export type DiscoveryScope = "all" | "needs" | "solutions";

// Raw domain record from GET /api/requests/{id}/decisions. ApproverId is a
// UserId value object and Decision an enum ordinal — normalize at render time.
export type AcceptanceDecision = {
  id: string;
  requestId: string;
  approverId: string | { value: string };
  decision: number | string;
  rationale: string;
  decidedAt: string;
};

/**
 * A proposed solution-to-idea link waiting on a reviewer.
 *
 * Declared here rather than imported: `PendingLinkResponse` exists in
 * `Momentum.Contracts/Models.cs` but TypeGen has never emitted it, so
 * `@momentum/contracts` has no such export and this package would not compile.
 * That gap is why the code-app stack derives its provider contract from generated
 * types with a compile-time field assertion — the same drift, caught rather than
 * discovered.
 *
 * Shape mirrors the C# record. Regenerating contracts should replace this.
 */
export type PendingLink = {
  requestId: string;
  requestTitle: string;
  solutionId: string;
  solutionTitle: string;
  relationship: string;
  addedBy: string;
  addedAt: string;
};

/**
 * Programme-level figures from GET /api/insights.
 *
 * Declared here rather than imported, for the same reason `PendingLink` is:
 * `InsightsResponse` exists in `Momentum.Contracts/Models.cs` and is marked for
 * export, but TypeGen has not been re-run, so `@momentum/contracts` has no such
 * type yet and this package would not compile. Shape mirrors the C# record.
 * Regenerating contracts should replace this block with an import.
 *
 * THE NULLS ARE LOAD-BEARING. A figure a backend cannot measure is null, and the
 * page renders that as "no data" rather than as zero — a confident zero is
 * indistinguishable from a real one.
 */
export type Insights = {
  generatedAt: string;
  ideas: { total: number; submitted30d: number; submittedPrior30d: number };
  approval: {
    medianDays: number | null;
    p90Days: number | null;
    sampleSize: number;
    source: string;
    staleCount: number;
    staleAfterDays: number;
  };
  voters: {
    distinct: number;
    totalVotes: number;
    population: number | null;
    populationSource: string | null;
    topTenShare: number | null;
  };
  engagement30d: {
    votes: number;
    comments: number;
    participation: number | null;
    adoptions: number;
  };
  solutions: { total: number; adopted: number };
  funnel: { label: string; value: number; detail?: string }[];
  /** Ranked highest-first, already truncated by the backend. */
  contributors: {
    id: string;
    name?: string | null;
    ideas: number;
    votes: number;
    comments: number;
    adoptions: number;
    total: number;
  }[];
};

export type ParticipationRequest = {
  id: string;
  itemType: string;
  itemId: string;
  requestedBy: string;
  message: string;
  status: string;
  createdAt: string;
};

/**
 * An audit record as the API returns it. `summary` is stored evidence, not UI
 * copy — feeds phrase themselves from `action` and `resourceType`. See
 * docs/reference/glossary.md.
 */
/** Optional display name resolved by the host; otherwise callers use the stored identity key. */
export type ActivityRecord = ActivityResponseItem & { actorName?: string | null };
