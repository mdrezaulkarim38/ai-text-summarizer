interface Props {
  text: string;
  processingTimeMs?: number;
}

export default function SummaryOutput({ text, processingTimeMs }: Props) {
  if (!text) return null;

  const copy = () => navigator.clipboard.writeText(text);
  const download = () => {
    const blob = new Blob([text], { type: "text/plain" });
    const url = URL.createObjectURL(blob);
    const a = document.createElement("a");
    a.href = url;
    a.download = "summary.txt";
    a.click();
    URL.revokeObjectURL(url);
  };

  return (
    <div className="card">
      <div className="card-header d-flex justify-content-between align-items-center">
        <strong>Summary</strong>
        <div>
          <button
            className="btn btn-sm btn-outline-primary me-2"
            onClick={copy}
          >
            Copy
          </button>
          <button
            className="btn btn-sm btn-outline-secondary"
            onClick={download}
          >
            Download
          </button>
        </div>
      </div>
      <div className="card-body" style={{ whiteSpace: "pre-wrap" }}>
        {text}
        {processingTimeMs !== undefined && (
          <div className="text-muted small mt-3">
            Generated in {processingTimeMs} ms
          </div>
        )}
      </div>
    </div>
  );
}
