/** Optional runtime environment configuration; unavailable values resolve to null. */
export interface EnvironmentProvider {
  /** Non-production banner label. Null in production, where the variable is blank. */
  getDesignation(): Promise<string | null>;
}
