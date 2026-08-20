import { useEffect, useState } from "react";
import { summarize, summarizeStream } from "./api/summarize";
import ExampleTexts from "./components/ExampleTexts";
import LoadingSpinner from "./components/LoadingSpinner";
import SummaryOutput from "./components/SummaryOutput";
import SummarizationControls from "./components/SummarizationControls";
import TextInputArea from "./components/TextInputArea";

function App() {
  const [text, setText] = useState("");
  const [maxLength, setMaxLength] = useState(150);
  const [format, setFormat] = useState<0 | 1>(0);
  const [output, setOutput] = useState("");
  const [processingTimeMs, setProcessingTimeMs] = useState<number>();
  const [loading, setLoading] = useState(false);
  const [useStreaming, setUseStreaming] = useState(true);
  const [error, setError] = useState("");
  const [darkMode, setDarkMode] = useState(() =>
    localStorage.getItem("theme") === "dark",
  );

  useEffect(() => {
    document.documentElement.setAttribute(
      "data-bs-theme",
      darkMode ? "dark" : "light",
    );
    localStorage.setItem("theme", darkMode ? "dark" : "light");
  }, [darkMode]);

  useEffect(() => {
    const onKeyDown = (e: KeyboardEvent) => {
      if (e.key === "Enter" && (e.ctrlKey || e.metaKey)) {
        e.preventDefault();
        handleSummarize();
      }
    };
    window.addEventListener("keydown", onKeyDown);
    return () => window.removeEventListener("keydown", onKeyDown);
  });

  const handleSummarize = async () => {
    if (!text.trim() || loading) return;
    setLoading(true);
    setError("");
    setOutput("");
    setProcessingTimeMs(undefined);
    const body = { text, maxLength, format };
    try {
      if (useStreaming) {
        await summarizeStream(body, (chunk) =>
          setOutput((prev) => prev + chunk),
        );
      } else {
        const res = await summarize(body);
        setOutput(res.summary);
        setProcessingTimeMs(res.processingTimeMs);
      }
    } catch (err) {
      setError(err instanceof Error ? err.message : "Something went wrong");
    } finally {
      setLoading(false);
    }
  };

  return (
    <div className="container py-5" style={{ maxWidth: 860 }}>
      <div className="d-flex justify-content-between align-items-start">
        <div>
          <h2 className="text-primary mb-1">AI Text Summarizer</h2>
          <p className="text-muted mb-0">
            Condense any text into a concise summary using local qwen3:8b.
          </p>
        </div>
        <div className="form-check form-switch">
          <input
            className="form-check-input"
            type="checkbox"
            id="dark-mode"
            checked={darkMode}
            onChange={(e) => setDarkMode(e.target.checked)}
          />
          <label className="form-check-label" htmlFor="dark-mode">
            Dark mode
          </label>
        </div>
      </div>

      <div className="my-3">
        <ExampleTexts onSelect={setText} />
      </div>

      <TextInputArea value={text} onChange={setText} />

      <div className="d-flex flex-wrap align-items-center justify-content-between gap-3 my-3">
        <SummarizationControls
          maxLength={maxLength}
          onMaxLength={setMaxLength}
          format={format}
          onFormat={setFormat}
        />
      </div>

      <div className="form-check form-switch mb-3">
        <input
          className="form-check-input"
          type="checkbox"
          id="stream-toggle"
          checked={useStreaming}
          onChange={(e) => setUseStreaming(e.target.checked)}
        />
        <label className="form-check-label" htmlFor="stream-toggle">
          Streaming (see words appear live)
        </label>
      </div>

      <button
        className="btn btn-primary px-4"
        onClick={handleSummarize}
        disabled={loading || !text.trim()}
      >
        {loading ? "Summarizing..." : "Summarize"}
      </button>
      <small className="text-muted ms-2">Ctrl+Enter to summarize</small>

      {error && <div className="alert alert-danger mt-3">{error}</div>}

      {loading && useStreaming && <LoadingSpinner />}

      <div className="mt-4">
        <SummaryOutput text={output} processingTimeMs={processingTimeMs} />
      </div>
    </div>
  );
}

export default App;