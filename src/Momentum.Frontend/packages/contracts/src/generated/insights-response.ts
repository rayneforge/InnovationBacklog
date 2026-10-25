/**
 * This is a TypeGen auto-generated file.
 * Any changes made to this file can be lost when this file is regenerated.
 */

import { IdeaFlowInsightsResponse } from "./idea-flow-insights-response";
import { ApprovalInsightsResponse } from "./approval-insights-response";
import { VoterInsightsResponse } from "./voter-insights-response";
import { EngagementInsightsResponse } from "./engagement-insights-response";
import { SolutionInsightsResponse } from "./solution-insights-response";
import { FunnelStageResponse } from "./funnel-stage-response";
import { ContributorInsightResponse } from "./contributor-insight-response";

export interface InsightsResponse {
    generatedAt: string;
    ideas: IdeaFlowInsightsResponse;
    approval: ApprovalInsightsResponse;
    voters: VoterInsightsResponse;
    engagement30d: EngagementInsightsResponse;
    solutions: SolutionInsightsResponse;
    funnel: FunnelStageResponse[];
    contributors: ContributorInsightResponse[];
}
