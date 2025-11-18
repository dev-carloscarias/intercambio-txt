import React, { useEffect, useState } from 'react';
import './SearchResults.scss';
import { Trans } from 'react-i18next';
import InfiniteScroll from 'react-infinite-scroll-component';
import { LoadingIndicator } from '@provider-portal/components';
import RequestingProviderItem, {
    RequestingProviderInterface
} from '../RequestingProviderItem/RequestingProviderItem';

export interface SearchResultsProps {
    providers: RequestingProviderInterface[];
    totalRows: number;
    handleLoadMore: () => void;
    id: string;
    containerId: string;
    isLoading: boolean;
    isChecked: (d: RequestingProviderInterface) => boolean;
    handleOnCheck: (i: RequestingProviderInterface) => void;
}

const SearchResults: React.FC<SearchResultsProps> = ({
    providers = [],
    totalRows = 0,
    handleLoadMore,
    id,
    containerId,
    isLoading,
    isChecked,
    handleOnCheck
}) => {
    const [hasMore, setHasMore] = useState(false);
    useEffect(() => {
        setHasMore(totalRows > providers?.length);
    }, [providers, totalRows]);

    return (
        <>
            <div className="dropdown-input-results">
                <div
                    className="dropdown-input-results-total"
                    id={`${id}-count`}
                >
                    <Trans
                        i18nKey={
                            'clinicalconsultation:requesting-provider.RESULTS'
                        }
                        values={{ totalRows: totalRows }}
                    >
                        Results {totalRows} specialists
                    </Trans>
                </div>
            </div>

            <InfiniteScroll
                scrollableTarget={containerId}
                dataLength={providers.length}
                next={handleLoadMore}
                hasMore={hasMore}
                loader={
                    <LoadingIndicator
                        id={`${id}-loading-indicator`}
                        loading={isLoading}
                        overlay={false}
                        inline={true}
                    />
                }
                endMessage={
                    <span className="dropdown-input-end-results">
                        <Trans
                            i18nKey={
                                'clinicalconsultation:create.END-OF-RESULTS'
                            }
                        >
                            End of results.
                        </Trans>
                    </span>
                }
            >
                {providers.map((p, i) => (
                    <div className="dropdown-input-item" key={i}>
                        <RequestingProviderItem
                            item={p}
                            onCheck={handleOnCheck}
                            checked={isChecked(p)}
                            id={i}
                        />
                    </div>
                ))}
            </InfiniteScroll>
        </>
    );
};

export default SearchResults;
