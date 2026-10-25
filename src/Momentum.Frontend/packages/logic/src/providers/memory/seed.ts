import type { ActivityEntry, Comment } from "../../domain/collaboration.js";
import type { Adoption, IdeaSolutionLink, Participation } from "../../domain/engagement.js";
import type { CurrentUser, UserRef } from "../../domain/identity.js";
import type { Idea } from "../../domain/idea.js";
import type { Solution } from "../../domain/solution.js";

/**
 * A dataset shaped like the one `Momentum.Service --seed-demo` writes: enough
 * ideas, solutions, people and engagement to make every surface look real, with at
 * least one row exercising each rule that is easy to get wrong.
 *
 * Dates are fixed strings rather than computed offsets so a snapshot test does not
 * change meaning overnight.
 */

export interface MemoryVote {
  targetKey: string;
  userId: string;
  createdAt: string;
}

export interface MemorySeed {
  currentUserId: string;
  users: UserRef[];
  ideas: Idea[];
  solutions: Solution[];
  votes: MemoryVote[];
  adoptions: Adoption[];
  comments: Comment[];
  participation: Participation[];
  links: IdeaSolutionLink[];
  activity: ActivityEntry[];
}

const USERS: UserRef[] = [
  { id: "u-avery", displayName: "Owen Haddad", email: "owen.haddad@museum-demo.example" },
  { id: "u-blake", displayName: "Noa Bekele", email: "noa.bekele@museum-demo.example" },
  { id: "u-casey", displayName: "Iris Serrano", email: "iris.serrano@museum-demo.example" },
  { id: "u-devin", displayName: "Felix Diallo", email: "felix.diallo@museum-demo.example" },
  { id: "u-ellis", displayName: "Amina Solberg", email: "amina.solberg@museum-demo.example" },
  { id: "u-frankie", displayName: "Jonah Sato", email: "jonah.sato@museum-demo.example" },
  { id: "u-harper", displayName: "Leila Novak", email: "leila.novak@museum-demo.example" },
  { id: "u-jordan", displayName: "Mira Duarte", email: "mira.duarte@museum-demo.example" },
];

function idea(
  id: string,
  title: string,
  description: string,
  submittedBy: string,
  status: Idea["status"],
  createdAt: string,
  tags: string[],
  overrides: Partial<Idea> = {},
): Idea {
  return {
    id,
    type: "Backlog",
    status,
    title,
    description,
    submittedBy,
    canonicalSolutionId: null,
    createdAt,
    updatedAt: createdAt,
    visibility: "Everyone",
    tags,
    ...overrides,
  };
}

function solution(
  id: string,
  title: string,
  description: string,
  submittedBy: string,
  type: Solution["type"],
  createdAt: string,
  tags: string[],
  overrides: Partial<Solution> = {},
): Solution {
  return {
    id,
    title,
    description,
    type,
    status: "Published",
    repositoryOwner: "",
    repositoryName: "",
    repositoryUrl: "",
    demoUrl: null,
    ownerId: submittedBy,
    useCount: 0,
    adoptedByProjects: [],
    createdAt,
    updatedAt: createdAt,
    publishedAt: createdAt,
    visibility: "Everyone",
    tags,
    ...overrides,
  };
}

const IDEAS: Idea[] = [
  idea("i-101", "Make quiet museum visits easier to plan",
    "Families need a sensory map, a quieter route and a clear arrival time.",
    "u-avery", "Accepted", "2026-05-11T09:12:00Z", ["visitor-access", "families"]),

  idea("i-102", "Write exhibition labels visitors understand",
    "Try plain-language object labels with visitors before agreeing the print proof.",
    "u-blake", "Accepted", "2026-05-18T14:40:00Z", ["interpretation", "exhibitions"],
    { canonicalSolutionId: "s-201" }),

  idea("i-103", "Record object condition before each move",
    "Photographs and handling notes are often missing when objects return from a loan.",
    "u-casey", "AwaitingApproval", "2026-06-09T08:05:00Z", ["collections-care", "handling"]),

  idea("i-104", "Make artwork loans easier to coordinate",
    "Registrars repeat packing, courier and insurance checks in different documents.",
    "u-devin", "Accepted", "2026-06-22T11:20:00Z", ["loans", "couriers"]),

  idea("i-105", "Create a calmer museum arrival",
    "Anxious visitors need clear signs and somewhere to pause before entering the galleries.",
    "u-ellis", "AwaitingApproval", "2026-07-13T16:32:00Z", ["visitor-welcome", "inclusion"]),

  idea("i-106", "Reduce energy waste after closing",
    "Lighting and heating sometimes run all night in empty galleries.",
    "u-frankie", "TriageRunning", "2026-07-27T10:00:00Z", ["archive", "retired"]),

  idea("i-107", "Take the collection to nearby villages",
    "Work with local hosts to bring a small travelling collection to people without transport.",
    "u-harper", "AwaitingApproval", "2026-08-08T13:45:00Z", ["community", "outreach"]),

  idea("i-108", "Track exhibition materials for exhibitions",
    "Plinths, display cases and signs are discarded because the next exhibition cannot find them.",
    "u-jordan", "Draft", "2026-08-25T09:00:00Z", ["sustainability"]),

  idea("i-109", "Replace every original object with a replica",
    "A replica-only display would simplify handling but remove the collection from public view.",
    "u-avery", "Rejected", "2026-08-31T15:10:00Z", ["collections"]),

  // Restricted: exercises the visibility filter and the owner exception.
  idea("i-110", "Review the Seabrook touring loan agreement",
    "Insurance limits and lender conditions stay restricted until the agreement is signed.",
    "u-casey", "AwaitingApproval", "2026-09-08T08:30:00Z", ["agreements"],
    { visibility: "Approvers" }),

  idea("i-111", "Plan an evening opening with local residents",
    "Ask residents which talks and activities would bring them to an evening museum visit.",
    "u-devin", "Accepted", "2026-09-20T12:00:00Z", ["events", "handling"]),
];

const SOLUTIONS: Solution[] = [
  solution("s-201", "Exhibition Label Guide", "Plain-language examples, translation notes and print specifications for galleries.",
    "u-blake", "Strategy", "2026-04-27T10:00:00Z", ["interpretation", "exhibitions"],
    { demoUrl: "https://museum-demo.example/guides/label-guide", useCount: 6 }),

  solution("s-202", "Quiet Hour Welcome Pack", "A sensory map, visitor guide and staff briefing for a calmer museum session.",
    "u-avery", "Strategy", "2026-05-13T09:30:00Z", ["visitor-access", "quiet-hours"], { useCount: 4 }),

  solution("s-203", "Artwork Loan Checklist", "Coordinate packing, insurance, courier arrangements and lender approvals.",
    "u-devin", "Strategy", "2026-06-29T15:00:00Z", ["loans", "couriers"], { useCount: 3 }),

  solution("s-204", "Object Condition Record", "Record photographs and short observations before an object moves.",
    "u-casey", "Strategy", "2026-07-09T11:15:00Z", ["collections-care", "handling"], { useCount: 2 }),

  solution("s-205", "Travelling Collection Planner", "Plan objects, local hosts and handling arrangements for village visits.",
    "u-harper", "Strategy", "2026-08-12T09:45:00Z", ["community", "outreach"], { useCount: 1 }),

  solution("s-206", "Reusable Exhibition Inventory", "Catalogue display cases, plinths and signs available for the next exhibition.",
    "u-jordan", "Strategy", "2026-08-27T14:20:00Z", ["sustainability"], { useCount: 5 }),

  // Retired: approved once, still visible so people can see what a team moved off.
  solution("s-207", "Printed Stock Ledger", "Retired paper records retained for historical reference.",
    "u-frankie", "Strategy", "2026-03-18T08:00:00Z", ["archive", "retired"],
    { status: "Retired", useCount: 2 }),
];

const VOTES: MemoryVote[] = [
  ...["u-blake", "u-casey", "u-devin", "u-ellis", "u-harper"].map((userId) => ({
    targetKey: "request:i-104", userId, createdAt: "2026-09-26T09:00:00Z",
  })),
  ...["u-avery", "u-jordan", "u-harper"].map((userId) => ({
    targetKey: "request:i-107", userId, createdAt: "2026-09-28T09:00:00Z",
  })),
  ...["u-blake", "u-frankie"].map((userId) => ({
    targetKey: "request:i-103", userId, createdAt: "2026-09-06T09:00:00Z",
  })),
  ...["u-avery", "u-blake", "u-casey", "u-devin"].map((userId) => ({
    targetKey: "solution:s-201", userId, createdAt: "2026-09-07T09:00:00Z",
  })),
  ...["u-ellis", "u-harper"].map((userId) => ({
    targetKey: "solution:s-206", userId, createdAt: "2026-09-17T09:00:00Z",
  })),
];

const ADOPTIONS: Adoption[] = [
  {
    "id": "museum-adoption-s-201-0",
    "solutionId": "s-201",
    "startedBy": "u-avery",
    "projectName": "Quiet Saturdays",
    "team": "Visitor Welcome",
    "status": "Using",
    "startedAt": "2026-08-05T09:00:00Z",
    "updatedAt": "2026-09-19T09:00:00Z",
    "completedAt": "2026-09-19T09:00:00Z"
  },
  {
    "id": "museum-adoption-s-201-1",
    "solutionId": "s-201",
    "startedBy": "u-blake",
    "projectName": "Autumn Exhibition",
    "team": "Gallery East",
    "status": "Using",
    "startedAt": "2026-08-05T09:00:00Z",
    "updatedAt": "2026-09-19T09:00:00Z",
    "completedAt": "2026-09-19T09:00:00Z"
  },
  {
    "id": "museum-adoption-s-201-2",
    "solutionId": "s-201",
    "startedBy": "u-casey",
    "projectName": "Village Visits",
    "team": "Collections Care",
    "status": "Using",
    "startedAt": "2026-08-05T09:00:00Z",
    "updatedAt": "2026-09-19T09:00:00Z",
    "completedAt": "2026-09-19T09:00:00Z"
  },
  {
    "id": "museum-adoption-s-201-3",
    "solutionId": "s-201",
    "startedBy": "u-devin",
    "projectName": "Spring Loans",
    "team": "Learning Studio",
    "status": "Exploring",
    "startedAt": "2026-08-05T09:00:00Z",
    "updatedAt": "2026-09-19T09:00:00Z",
    "completedAt": null
  },
  {
    "id": "museum-adoption-s-202-0",
    "solutionId": "s-202",
    "startedBy": "u-avery",
    "projectName": "Quiet Saturdays",
    "team": "Visitor Welcome",
    "status": "Using",
    "startedAt": "2026-08-05T09:00:00Z",
    "updatedAt": "2026-09-19T09:00:00Z",
    "completedAt": "2026-09-19T09:00:00Z"
  },
  {
    "id": "museum-adoption-s-202-1",
    "solutionId": "s-202",
    "startedBy": "u-blake",
    "projectName": "Autumn Exhibition",
    "team": "Gallery East",
    "status": "Using",
    "startedAt": "2026-08-05T09:00:00Z",
    "updatedAt": "2026-09-19T09:00:00Z",
    "completedAt": "2026-09-19T09:00:00Z"
  },
  {
    "id": "museum-adoption-s-202-2",
    "solutionId": "s-202",
    "startedBy": "u-casey",
    "projectName": "Village Visits",
    "team": "Collections Care",
    "status": "Using",
    "startedAt": "2026-08-05T09:00:00Z",
    "updatedAt": "2026-09-19T09:00:00Z",
    "completedAt": "2026-09-19T09:00:00Z"
  },
  {
    "id": "museum-adoption-s-202-3",
    "solutionId": "s-202",
    "startedBy": "u-devin",
    "projectName": "Spring Loans",
    "team": "Learning Studio",
    "status": "Using",
    "startedAt": "2026-08-05T09:00:00Z",
    "updatedAt": "2026-09-19T09:00:00Z",
    "completedAt": "2026-09-19T09:00:00Z"
  },
  {
    "id": "museum-adoption-s-202-4",
    "solutionId": "s-202",
    "startedBy": "u-ellis",
    "projectName": "Quiet Saturdays",
    "team": "Archives",
    "status": "Using",
    "startedAt": "2026-08-05T09:00:00Z",
    "updatedAt": "2026-09-19T09:00:00Z",
    "completedAt": "2026-09-19T09:00:00Z"
  },
  {
    "id": "museum-adoption-s-202-5",
    "solutionId": "s-202",
    "startedBy": "u-frankie",
    "projectName": "Autumn Exhibition",
    "team": "Conservation",
    "status": "Using",
    "startedAt": "2026-08-05T09:00:00Z",
    "updatedAt": "2026-09-19T09:00:00Z",
    "completedAt": "2026-09-19T09:00:00Z"
  },
  {
    "id": "museum-adoption-s-202-6",
    "solutionId": "s-202",
    "startedBy": "u-harper",
    "projectName": "Village Visits",
    "team": "Touring Exhibitions",
    "status": "Using",
    "startedAt": "2026-08-05T09:00:00Z",
    "updatedAt": "2026-09-19T09:00:00Z",
    "completedAt": "2026-09-19T09:00:00Z"
  },
  {
    "id": "museum-adoption-s-202-7",
    "solutionId": "s-202",
    "startedBy": "u-jordan",
    "projectName": "Spring Loans",
    "team": "Community Programmes",
    "status": "Exploring",
    "startedAt": "2026-08-05T09:00:00Z",
    "updatedAt": "2026-09-19T09:00:00Z",
    "completedAt": null
  },
  {
    "id": "museum-adoption-s-203-0",
    "solutionId": "s-203",
    "startedBy": "u-avery",
    "projectName": "Quiet Saturdays",
    "team": "Visitor Welcome",
    "status": "Using",
    "startedAt": "2026-08-05T09:00:00Z",
    "updatedAt": "2026-09-19T09:00:00Z",
    "completedAt": "2026-09-19T09:00:00Z"
  },
  {
    "id": "museum-adoption-s-203-1",
    "solutionId": "s-203",
    "startedBy": "u-blake",
    "projectName": "Autumn Exhibition",
    "team": "Gallery East",
    "status": "Using",
    "startedAt": "2026-08-05T09:00:00Z",
    "updatedAt": "2026-09-19T09:00:00Z",
    "completedAt": "2026-09-19T09:00:00Z"
  },
  {
    "id": "museum-adoption-s-203-2",
    "solutionId": "s-203",
    "startedBy": "u-casey",
    "projectName": "Village Visits",
    "team": "Collections Care",
    "status": "Using",
    "startedAt": "2026-08-05T09:00:00Z",
    "updatedAt": "2026-09-19T09:00:00Z",
    "completedAt": "2026-09-19T09:00:00Z"
  },
  {
    "id": "museum-adoption-s-203-3",
    "solutionId": "s-203",
    "startedBy": "u-devin",
    "projectName": "Spring Loans",
    "team": "Learning Studio",
    "status": "Using",
    "startedAt": "2026-08-05T09:00:00Z",
    "updatedAt": "2026-09-19T09:00:00Z",
    "completedAt": "2026-09-19T09:00:00Z"
  },
  {
    "id": "museum-adoption-s-203-4",
    "solutionId": "s-203",
    "startedBy": "u-ellis",
    "projectName": "Quiet Saturdays",
    "team": "Archives",
    "status": "Exploring",
    "startedAt": "2026-08-05T09:00:00Z",
    "updatedAt": "2026-09-19T09:00:00Z",
    "completedAt": null
  },
  {
    "id": "museum-adoption-s-204-0",
    "solutionId": "s-204",
    "startedBy": "u-avery",
    "projectName": "Quiet Saturdays",
    "team": "Visitor Welcome",
    "status": "Using",
    "startedAt": "2026-08-05T09:00:00Z",
    "updatedAt": "2026-09-19T09:00:00Z",
    "completedAt": "2026-09-19T09:00:00Z"
  },
  {
    "id": "museum-adoption-s-204-1",
    "solutionId": "s-204",
    "startedBy": "u-blake",
    "projectName": "Autumn Exhibition",
    "team": "Gallery East",
    "status": "Exploring",
    "startedAt": "2026-08-05T09:00:00Z",
    "updatedAt": "2026-09-19T09:00:00Z",
    "completedAt": null
  },
  {
    "id": "museum-adoption-s-205-0",
    "solutionId": "s-205",
    "startedBy": "u-avery",
    "projectName": "Quiet Saturdays",
    "team": "Visitor Welcome",
    "status": "Using",
    "startedAt": "2026-08-05T09:00:00Z",
    "updatedAt": "2026-09-19T09:00:00Z",
    "completedAt": "2026-09-19T09:00:00Z"
  },
  {
    "id": "museum-adoption-s-205-1",
    "solutionId": "s-205",
    "startedBy": "u-blake",
    "projectName": "Autumn Exhibition",
    "team": "Gallery East",
    "status": "Using",
    "startedAt": "2026-08-05T09:00:00Z",
    "updatedAt": "2026-09-19T09:00:00Z",
    "completedAt": "2026-09-19T09:00:00Z"
  },
  {
    "id": "museum-adoption-s-205-2",
    "solutionId": "s-205",
    "startedBy": "u-casey",
    "projectName": "Village Visits",
    "team": "Collections Care",
    "status": "Exploring",
    "startedAt": "2026-08-05T09:00:00Z",
    "updatedAt": "2026-09-19T09:00:00Z",
    "completedAt": null
  },
  {
    "id": "museum-adoption-s-206-0",
    "solutionId": "s-206",
    "startedBy": "u-avery",
    "projectName": "Quiet Saturdays",
    "team": "Visitor Welcome",
    "status": "Using",
    "startedAt": "2026-08-05T09:00:00Z",
    "updatedAt": "2026-09-19T09:00:00Z",
    "completedAt": "2026-09-19T09:00:00Z"
  },
  {
    "id": "museum-adoption-s-206-1",
    "solutionId": "s-206",
    "startedBy": "u-blake",
    "projectName": "Autumn Exhibition",
    "team": "Gallery East",
    "status": "Using",
    "startedAt": "2026-08-05T09:00:00Z",
    "updatedAt": "2026-09-19T09:00:00Z",
    "completedAt": "2026-09-19T09:00:00Z"
  },
  {
    "id": "museum-adoption-s-206-2",
    "solutionId": "s-206",
    "startedBy": "u-casey",
    "projectName": "Village Visits",
    "team": "Collections Care",
    "status": "Using",
    "startedAt": "2026-08-05T09:00:00Z",
    "updatedAt": "2026-09-19T09:00:00Z",
    "completedAt": "2026-09-19T09:00:00Z"
  },
  {
    "id": "museum-adoption-s-206-3",
    "solutionId": "s-206",
    "startedBy": "u-devin",
    "projectName": "Spring Loans",
    "team": "Learning Studio",
    "status": "Using",
    "startedAt": "2026-08-05T09:00:00Z",
    "updatedAt": "2026-09-19T09:00:00Z",
    "completedAt": "2026-09-19T09:00:00Z"
  },
  {
    "id": "museum-adoption-s-206-4",
    "solutionId": "s-206",
    "startedBy": "u-ellis",
    "projectName": "Quiet Saturdays",
    "team": "Archives",
    "status": "Using",
    "startedAt": "2026-08-05T09:00:00Z",
    "updatedAt": "2026-09-19T09:00:00Z",
    "completedAt": "2026-09-19T09:00:00Z"
  },
  {
    "id": "museum-adoption-s-206-5",
    "solutionId": "s-206",
    "startedBy": "u-frankie",
    "projectName": "Autumn Exhibition",
    "team": "Conservation",
    "status": "Exploring",
    "startedAt": "2026-08-05T09:00:00Z",
    "updatedAt": "2026-09-19T09:00:00Z",
    "completedAt": null
  }
];

const COMMENTS: Comment[] = [
  { id: "c-401", subjectId: "i-104", subjectType: "Idea", authorId: "u-blake", body: "The loan checklist already brings the courier and insurance checks together.",
    attachments: [], createdAt: "2026-06-25T10:30:00Z" },
  { id: "c-402", subjectId: "i-104", subjectType: "Idea", authorId: "u-avery", body: "Can the collections team confirm the lender conditions are current?",
    attachments: [], createdAt: "2026-06-26T08:00:00Z" },
  // The audience that must never reach a submitter on any read path.
  { id: "c-403", subjectId: "i-104", subjectType: "Idea", authorId: "u-agent", body: "Review note: the loan checklist addresses this need; propose a link.",
    attachments: [], createdAt: "2026-06-26T08:05:00Z" },
  { id: "c-404", subjectId: "s-201", subjectType: "Solution", authorId: "u-harper", body: "We tested these labels in Gallery East and visitors understood the shorter wording.",
    attachments: [], createdAt: "2026-07-22T13:00:00Z" },
  { id: "c-405", subjectId: "i-110", subjectType: "Idea", authorId: "u-avery", body: "Hold until the negotiation closes.",
    attachments: [], createdAt: "2026-09-09T09:00:00Z" },
];

const PARTICIPATION: Participation[] = [
  { id: "p-501", itemType: "Idea", itemId: "i-107", requestedBy: "u-jordan",
    message: "I have done this at a previous employer and would like to help.",
    status: "Proposed", decidedBy: null, rationale: null,
    createdAt: "2026-09-11T09:00:00Z", updatedAt: "2026-09-11T09:00:00Z", decidedAt: null },
  { id: "p-502", itemType: "Solution", itemId: "s-206", requestedBy: "u-ellis",
    message: "Happy to catalogue the display cases before the next exhibition.", status: "Accepted",
    decidedBy: "u-jordan", rationale: "The inventory needs a curator for its first review.",
    createdAt: "2026-09-01T09:00:00Z", updatedAt: "2026-09-03T09:00:00Z",
    decidedAt: "2026-09-03T09:00:00Z" },
];

const LINKS: IdeaSolutionLink[] = [
  { ideaId: "i-102", solutionId: "s-201", addedBy: "u-avery", addedAt: "2026-05-20T09:00:00Z" },
  { ideaId: "i-101", solutionId: "s-202", addedBy: "u-avery", addedAt: "2026-05-14T09:00:00Z" },
  { ideaId: "i-104", solutionId: "s-203", addedBy: "u-avery", addedAt: "2026-06-26T09:00:00Z" },
];

const ACTIVITY: ActivityEntry[] = [
  { id: "act-601", action: "solution.published", resourceType: "solution", resourceId: "s-206",
    subjectId: "s-206", actorType: "User", actorId: "u-jordan",
    summary: "Reusable Exhibition Inventory", audience: "SubmitterAndApprovers",
    occurredAt: "2026-08-27T14:20:00Z" },
  { id: "act-602", action: "adoption.started", resourceType: "solution", resourceId: "s-201",
    subjectId: "s-201", actorType: "User", actorId: "u-ellis",
    summary: "Village Visits", audience: "SubmitterAndApprovers", occurredAt: "2026-09-14T09:00:00Z" },
  { id: "act-603", action: "request.created", resourceType: "request", resourceId: "i-111",
    subjectId: "i-111", actorType: "User", actorId: "u-devin",
    summary: "Plan an evening opening with local residents", audience: "SubmitterAndApprovers",
    occurredAt: "2026-09-20T12:00:00Z" },
  { id: "act-604", action: "vote.added", resourceType: "request", resourceId: "i-104",
    subjectId: "i-104", actorType: "User", actorId: "u-harper",
    summary: "Make artwork loans easier to coordinate", audience: "SubmitterAndApprovers",
    occurredAt: "2026-09-26T09:00:00Z" },
  { id: "act-605", action: "item.visibilityChanged", resourceType: "request", resourceId: "i-110",
    subjectId: "i-110", actorType: "User", actorId: "u-avery",
    summary: "Review the Seabrook touring loan agreement", audience: "ApproversOnly",
    occurredAt: "2026-09-08T08:35:00Z" },
];

for (const record of SOLUTIONS) {
  const adoptions = ADOPTIONS.filter(row => row.solutionId === record.id);
  record.useCount = adoptions.length;
  record.adoptedByProjects = [...new Set(adoptions.map(row => row.projectName))];
}

export function defaultSeed(): MemorySeed {
  return {
    currentUserId: "u-avery",
    users: USERS,
    ideas: IDEAS,
    solutions: SOLUTIONS,
    votes: VOTES,
    adoptions: ADOPTIONS,
    comments: COMMENTS,
    participation: PARTICIPATION,
    links: LINKS,
    activity: ACTIVITY,
  };
}
