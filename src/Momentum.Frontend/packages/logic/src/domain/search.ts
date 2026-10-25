import type {
  RequestSummaryEntry,
  SearchResponseItem,
  SolutionSummaryEntry,
} from "@momentum/contracts";
import type { Assert, FieldsExistOn } from "./common.js";
import type { HubItemType, ItemVisibility } from "./enums.js";

/**
 * One row in a unified search result.
 *
 * Every one of `subtype`, `submittedBy`, `visibility` and `tags` is optional in C#
 * and non-optional after TypeGen. They are genuinely absent on some paths — a
 * search row with no `submittedBy` is what made "Shared by" render an em dash for
 * every item — so the domain restores the nulls.
 */
export interface SearchItem {
  itemType: HubItemType;
  itemId: string;
  title: string;
  description: string;
  status: string;
  canonicalSolutionId: string | null;
  repositoryUrl: string | null;
  team: string | null;
  createdAt: string;
  updatedAt: string;
  /** Idea kind or solution kind — the second half of the "IDEA · …" eyebrow. */
  subtype: string | null;
  submittedBy: string | null;
  visibility: ItemVisibility | null;
  tags: string[];
}

export type SearchItemMatchesWire = Assert<FieldsExistOn<SearchItem, SearchResponseItem>>;

export interface SearchResult {
  items: SearchItem[];
  totalCount: number;
}

export interface SearchQuery {
  query: string;
  skip?: number;
  take?: number;
}

// ---------------------------------------------------------------------------
// Engagement rollups
// ---------------------------------------------------------------------------

/** Precomputed engagement counts keyed by item id, avoiding per-item reads. */
export interface IdeaRollup {
  votes: number;
  votes30d: number;
  votedByMe: boolean;
  linkedSolutions: number;
  contributors: number;
  comments: number;
}

export type IdeaRollupMatchesWire = Assert<FieldsExistOn<IdeaRollup, RequestSummaryEntry>>;

export interface SolutionRollup {
  adoptions: number;
  teams: number;
  linkedNeeds: number;
  activeUses: number;
  completedUses: number;
  votes: number;
  votedByMe: boolean;
  comments: number;
}

export type SolutionRollupMatchesWire = Assert<FieldsExistOn<SolutionRollup, SolutionSummaryEntry>>;

/** Rollups arrive keyed by item id so a list can look each row up in one pass. */
export type RollupMap<T> = Record<string, T | undefined>;
