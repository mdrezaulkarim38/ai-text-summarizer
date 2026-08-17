export interface SummarizeRequest {
  text: string;
  maxLength: number;
  format: 0 | 1;
}

export interface SummarizeResponse {
  summary: string;
  model: string;
  originalWordCount: number;
  summaryWordCount: number;
  processingTimeMs: number;
}

export async function summarize(
  body: SummarizeRequest,
): Promise<SummarizeResponse> {
  const res = await fetch("/api/summarization", {
    method: "POST",
    headers: { "Content-Type": "application/json" },
    body: JSON.stringify(body),
  });
  if (!res.ok) throw new Error(`HTTP ${res.status}`);
  return res.json();
}

export async function summarizeStream(
  body: SummarizeRequest,
  onChunk: (text: string) => void,
): Promise<void> {
  const res = await fetch("/api/summarization/stream", {
    method: "POST",
    headers: { "Content-Type": "application/json" },
    body: JSON.stringify(body),
  });
  if (!res.ok) throw new Error(`HTTP ${res.status}`);

  const reader = res.body!.getReader();
  const decoder = new TextDecoder();
  let buffer = "";

  while (true) {
    const { done, value } = await reader.read();
    if (done) break;
    buffer += decoder.decode(value, { stream: true });

    const lines = buffer.split("\n\n");
    buffer = lines.pop() ?? "";
    for (const line of lines) {
      if (!line.startsWith("data: ")) continue;
      const payload = line.slice(6);
      if (payload === "[DONE]") return;
      onChunk(JSON.parse(payload).content);
    }
  }
}
