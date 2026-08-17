interface Props {
  maxLength: number;
  onMaxLength: (v: number) => void;
  format: 0 | 1;
  onFormat: (v: 0 | 1) => void;
}

export default function SummarizationControls({
  maxLength,
  onMaxLength,
  format,
  onFormat,
}: Props) {
  return (
    <div className="row g-3 align-items-center">
      <div className="col-md-6">
        <label className="form-label">
          Max summary length: <strong>{maxLength}</strong> words
        </label>
        <input
          type="range"
          className="form-range"
          min={20}
          max={300}
          step={10}
          value={maxLength}
          onChange={(e) => onMaxLength(Number(e.target.value))}
        />
      </div>
      <div className="col-md-6">
        <span className="form-label d-block">Format</span>
        <div className="form-check form-check-inline">
          <input
            className="form-check-input"
            type="radio"
            id="fmt-paragraph"
            checked={format === 0}
            onChange={() => onFormat(0)}
          />
          <label className="form-check-label" htmlFor="fmt-paragraph">
            Paragraph
          </label>
        </div>
        <div className="form-check form-check-inline">
          <input
            className="form-check-input"
            type="radio"
            id="fmt-bullets"
            checked={format === 1}
            onChange={() => onFormat(1)}
          />
          <label className="form-check-label" htmlFor="fmt-bullets">
            Bullet points
          </label>
        </div>
      </div>
    </div>
  );
}
