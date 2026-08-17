interface Props {
  value: string;
  onChange: (v: string) => void;
}

export default function TextInputArea({ value, onChange }: Props) {
  const words = value.trim() ? value.trim().split(/\s+/).length : 0;

  return (
    <div>
      <label className="form-label">Text to summarize</label>
      <textarea
        className="form-control"
        rows={8}
        placeholder="Paste or type the content you want to summarize..."
        value={value}
        onChange={(e) => onChange(e.target.value)}
      />
      <small className="text-muted">
        {value.length} chars · {words} words
      </small>
    </div>
  );
}
