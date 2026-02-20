(() => {
    const translationState = {
        languageCode: 'tr',
        languageName: 'Turkish',
        textNodeMap: new WeakMap()
    };

    const attributeMap = [
        { name: 'placeholder', dataset: 'aiOriginalPlaceholder' },
        { name: 'title', dataset: 'aiOriginalTitle' },
        { name: 'aria-label', dataset: 'aiOriginalAriaLabel' },
        { name: 'data-city', dataset: 'aiOriginalCity' }
    ];

    const getTextNodes = () => {
        const nodes = [];
        const walker = document.createTreeWalker(document.body, NodeFilter.SHOW_TEXT, {
            acceptNode(node) {
                if (!node.parentElement) {
                    return NodeFilter.FILTER_REJECT;
                }

                const tag = node.parentElement.tagName;
                if (['SCRIPT', 'STYLE', 'NOSCRIPT'].includes(tag)) {
                    return NodeFilter.FILTER_REJECT;
                }

                const text = node.textContent?.trim();
                if (!text) {
                    return NodeFilter.FILTER_REJECT;
                }

                return NodeFilter.FILTER_ACCEPT;
            }
        });

        let current;
        while ((current = walker.nextNode())) {
            nodes.push(current);
        }

        return nodes;
    };

    const collectTexts = () => {
        const textNodes = getTextNodes();
        const texts = [];
        const textNodeOriginals = [];

        textNodes.forEach(node => {
            const stored = translationState.textNodeMap.get(node);
            const original = stored ?? node.textContent;
            if (!stored) {
                translationState.textNodeMap.set(node, original);
            }

            texts.push(original);
            textNodeOriginals.push(node);
        });

        const attributeEntries = [];
        document.querySelectorAll('[placeholder],[title],[aria-label],[data-city]').forEach(element => {
            attributeMap.forEach(attribute => {
                if (!element.hasAttribute(attribute.name)) {
                    return;
                }

                const originalKey = attribute.dataset;
                const stored = element.dataset[originalKey];
                const original = stored ?? element.getAttribute(attribute.name);
                if (!stored) {
                    element.dataset[originalKey] = original ?? '';
                }

                texts.push(original ?? '');
                attributeEntries.push({ element, attribute: attribute.name, originalKey });
            });
        });

        return { texts, textNodes: textNodeOriginals, attributeEntries };
    };

    const applyTranslations = (translations, textNodes, attributeEntries) => {
        let index = 0;
        textNodes.forEach(node => {
            node.textContent = translations[index++] ?? node.textContent;
        });

        attributeEntries.forEach(entry => {
            const value = translations[index++] ?? entry.element.getAttribute(entry.attribute);
            entry.element.setAttribute(entry.attribute, value ?? '');
        });
    };

    const restoreOriginals = () => {
        const textNodes = getTextNodes();
        textNodes.forEach(node => {
            const original = translationState.textNodeMap.get(node);
            if (original) {
                node.textContent = original;
            }
        });

        document.querySelectorAll('[data-ai-original-placeholder],[data-ai-original-title],[data-ai-original-aria-label],[data-ai-original-city]').forEach(element => {
            attributeMap.forEach(attribute => {
                const originalValue = element.dataset[attribute.dataset];
                if (originalValue !== undefined) {
                    element.setAttribute(attribute.name, originalValue);
                }
            });
        });
    };

    const translatePage = async (languageCode, languageName) => {
        translationState.languageCode = languageCode;
        translationState.languageName = languageName;
        document.documentElement.lang = languageCode;

        if (languageCode === 'tr') {
            restoreOriginals();
            return;
        }

        const { texts, textNodes, attributeEntries } = collectTexts();
        if (!texts.length) {
            return;
        }

        const response = await fetch('/api/ai/translate', {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify({ targetLanguage: languageName, texts })
        });

        if (!response.ok) {
            console.error('Translation request failed.');
            return;
        }

        const data = await response.json();
        applyTranslations(data.translations || [], textNodes, attributeEntries);
    };

    const setActiveLanguageLabel = (label) => {
        const labelElement = document.getElementById('currentLanguageLabel');
        if (labelElement) {
            labelElement.textContent = label;
        }
    };

    document.addEventListener('DOMContentLoaded', () => {
        document.querySelectorAll('[data-language-code]').forEach(item => {
            item.addEventListener('click', async event => {
                event.preventDefault();
                const languageCode = item.dataset.languageCode;
                const languageName = item.dataset.languageName;
                const label = item.dataset.languageLabel ?? item.textContent?.trim();
                setActiveLanguageLabel(label ?? '');
                try {
                    await translatePage(languageCode, languageName);
                } catch (error) {
                    console.error('Translation error', error);
                }
            });
        });
    });

    window.aiTranslationState = translationState;
})();