import type {
  ActivityResponseItem,
  AttachmentResponse,
  CommentResponse,
} from "@momentum/contracts";
import type { Assert, FieldsExistOn, FieldsExistOnExcept } from "./common.js";
import type { ActorType, HubItemType } from "./enums.js";

// ---------------------------------------------------------------------------
// Attachments
// ---------------------------------------------------------------------------

export interface Attachment {
  id: string;
  fileName: string;
  contentType: string;
  length: number;
  /** Optional attachment URL supplied by a host; otherwise use /api/attachments/{id}. */
  url?: string | null;
}

// `url` is supplied by the adapter and has no wire counterpart; every other field
// still has to exist on the generated DTO.
export type AttachmentMatchesWire = Assert<
  FieldsExistOnExcept<Attachment, AttachmentResponse, "url">
>;

/** Uploads travel as base64 JSON so they use the same transport as every other call. */
export interface UploadAttachmentInput {
  fileName: string;
  contentType?: string;
  contentBase64: string;
}

export const MAX_ATTACHMENT_BYTES = 10 * 1024 * 1024;

// ---------------------------------------------------------------------------
// Comments
// ---------------------------------------------------------------------------

/** Public comment projection. The backend enforces item visibility and comment audience. */
export interface Comment {
  id: string;
  subjectId: string;
  subjectType: HubItemType;
  authorId: string;
  body: string;
  attachments: Attachment[];
  createdAt: string;
}

export type CommentMatchesWire = Assert<FieldsExistOn<Comment, CommentResponse>>;

export interface AddCommentInput {
  subjectId: string;
  subjectType: HubItemType;
  body: string;
  attachmentIds?: string[];
}

// ---------------------------------------------------------------------------
// Activity
// ---------------------------------------------------------------------------

/**
 * One entry in the user-facing activity feed.
 *
 * `action` is a stable key such as `vote.added`. UI wording is derived from it,
 * never from `summary` — feeds that rendered stored prose could not be restated
 * when the vocabulary changed, and rows written before a wording change would keep
 * the old phrasing forever.
 */
export interface ActivityEntry {
  id: string;
  action: string;
  resourceType: string;
  resourceId: string;
  subjectId: string;
  actorType: ActorType;
  actorId: string;
  /** Optional display name resolved by the host; otherwise callers use the stored identity key. */
  actorName?: string | null;
  summary: string;
  audience: string;
  occurredAt: string;
}

// actorName is resolved by the adapter and has no wire counterpart; every other
// field still has to exist on the generated DTO.
export type ActivityMatchesWire = Assert<
  FieldsExistOnExcept<ActivityEntry, ActivityResponseItem, "actorName">
>;

export interface ActivityQuery {
  take?: number;
  subjectId?: string;
  subjectType?: HubItemType;
}
