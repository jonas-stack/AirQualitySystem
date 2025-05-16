// components/CompactMarkdownRenderer.tsx
import React from "react";
import ReactMarkdown from "react-markdown";
import remarkGfm from "remark-gfm";
import rehypeHighlight from "rehype-highlight";
import "highlight.js/styles/github.css"; // Or choose another theme

type Props = {
    content: string;
};

const CompactMarkdownRenderer: React.FC<Props> = ({ content }) => {
    return (
        <div className="prose prose-sm max-w-none text-sm leading-relaxed text-gray-700 dark:text-gray-300">
            <ReactMarkdown
                children={content}
                remarkPlugins={[remarkGfm]}
                rehypePlugins={[rehypeHighlight]}
            />
        </div>
    );
};

export default CompactMarkdownRenderer;