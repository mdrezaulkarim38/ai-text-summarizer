interface Props {
  onSelect: (text: string) => void;
}

const EXAMPLES: { label: string; text: string }[] = [
  {
    label: "Article: Climate Change",
    text: "Climate change is one of the most urgent challenges facing humanity. Global temperatures are rising, leading to more frequent hurricanes, droughts and floods that destroy homes and farmland. Scientists agree the main cause is burning fossil fuels like coal, oil and gas. The good news is that renewable energy such as solar and wind power is now cheaper than ever, and many countries are setting ambitious goals to cut carbon emissions by 2030. Experts say that coordinated action by governments, businesses and individuals can still limit warming and protect the planet for future generations.",
  },
  {
    label: "Business Email",
    text: "Dear Team, I am writing to summarize the outcome of yesterday's planning meeting. We agreed to launch the new customer portal on the first of next month. The development team has committed to completing the remaining integration work by the end of this week, and the QA team will run a full regression cycle next week. Marketing will prepare the launch announcement and customer communication template. Please review the attached timeline and flag any concerns by Friday so we can address them before the final launch.",
  },
  {
    label: "Tech News",
    text: "The technology industry is evolving at an unprecedented pace. Artificial intelligence assistants are now integrated into operating systems, office suites, and even cars, changing how people work and travel. Cloud providers have made it possible for small startups to deploy globally scaled applications without owning any physical servers. At the same time, concerns about data privacy and the environmental cost of training large models are growing. Companies are responding by publishing transparency reports, investing in energy-efficient hardware, and exploring federated learning techniques that keep user data on personal devices.",
  },
  {
    label: "Meeting Notes",
    text: "Weekly project sync held on Monday. Status update: the API integration is 80% complete and blocking on the payment gateway credentials from the finance team. Frontend design review is scheduled for Wednesday. Two new bugs were reported in the checkout flow - both assigned to the developer team with medium priority. Decision made: postpone the mobile app beta by one week to align with the web launch. Action items: finance to provide credentials by Tuesday, QA to prepare test data for the new refund flow, and product owner to confirm the final acceptance criteria for the reporting dashboard.",
  },
  {
    label: "History Passage",
    text: "The Industrial Revolution transformed human society in ways that still shape our world today. Beginning in Britain around 1760, it introduced machines that could spin cotton, weave cloth, and pump water from mines far faster than human or animal power. Steam engines powered factories and locomotives, collapsing travel times from weeks to hours. Cities swelled as rural workers migrated in search of factory jobs, creating new urban centers with problems of overcrowding and poor sanitation. The factory system created a new industrial working class, and working conditions were harsh, eventually sparking labor movements that fought for safer workplaces and shorter hours.",
  },
];

export default function ExampleTexts({ onSelect }: Props) {
  return (
    <div className="d-flex align-items-center gap-2">
      <label className="form-label mb-0" htmlFor="example-texts">
        Try an example:
      </label>
      <select
        id="example-texts"
        className="form-select form-select-sm"
        style={{ maxWidth: 260 }}
        defaultValue=""
        onChange={(e) => {
          const example = EXAMPLES.find((x) => x.label === e.target.value);
          if (example) onSelect(example.text);
        }}
      >
        <option value="" disabled>
          Choose a sample text...
        </option>
        {EXAMPLES.map((example) => (
          <option key={example.label} value={example.label}>
            {example.label}
          </option>
        ))}
      </select>
    </div>
  );
}