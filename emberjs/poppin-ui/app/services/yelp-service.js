import Service, { inject as injectService } from '@ember/service';
import HttpResources from '../utils/http-resources';
import YelpSearchEntities from '../classes/yelp-search-entities';

export default class YelpService extends Service {
	@injectService apiService

	getYelpCategories() {
		return this.apiService.getJSON('yelp-categories.json');
	}

	/**
	 * Runs Yelp's Business Match search when a Poppin location exists
	 * @param {String} locId Poppin location ID
	 */
	getLocMatch(locId) {
		return this.apiService.request({
			resource: HttpResources.getYelpMatchLocId,
			body: { locId }
		});
	}

	/**
	 * Runs Yelp's Business Match search
	 * @param {Object} searchParams
	 */
	getYelpMatch(searchParams) {
		try {
			const params = new YelpSearchEntities.YelpBusinessMatchSearchParams(searchParams);
			return this.apiService.request({
				resource: HttpResources.getYelpMatch,
				body: params
			});
		} catch (e) {
			console.error(e);
		}
 	}

	/**
	 * @param {String} yelpId 
	 */
	getYelpBusiness(yelpId) {
		return this.apiService.request({
			resource: HttpResources.getYelpBusiness,
			body: { yelpId }
		});
	}

	/**
	 * Runs Yelp's Business Search
	 * @param {Object} searchParams 
	 */
	getYelpBusinessSearch(searchParams) {
		try {
			const params = new YelpSearchEntities.YelpBusinessSearchParams(searchParams);
			return this.apiService.request({
				resource: HttpResources.getYelpBusinessSearch,
				body: params
			});
		} catch (e) {
			console.error(e);
		}
	}
}
