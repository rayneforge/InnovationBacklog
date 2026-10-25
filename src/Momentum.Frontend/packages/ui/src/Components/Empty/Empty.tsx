import type React from "react";
import styles from "./Empty.module.scss";

export function Empty({ text }: { text: string }): React.ReactElement {
  return (
    <div className={styles.empty}>
      <strong>Nothing here yet</strong>
      <p>{text}</p>
    </div>
  );
}

export function Pending({ text }: { text: string }): React.ReactElement {
  return (
    <div className={styles.pending} role="status" aria-live="polite">
      <span className={styles.spinner} aria-hidden="true" />
      <p>{text}</p>
    </div>
  );
}

export function ContextualEmpty({
  title,
  text,
}: {
  title: string;
  text: string;
}): React.ReactElement {
  return (
    <div className={styles.contextual}>
      <strong>{title}</strong>
      <p>{text}</p>
    </div>
  );
}
