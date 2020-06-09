import Service, { inject as injectService } from '@ember/service';
import HttpResources from '../utils/http-resources';
import YelpSearchEntities from '../classes/yelp-search-entities';
import { Promise } from 'rsvp';

export default class LocationsService extends Service {
	@injectService apiService;

	createNewLocation(location) {
		return this.apiService.request({
			resource: HttpResources.createNewLocation,
			body: location
		});
	}

	updateLocation(location) {
		return this.apiService.request({
			resource: HttpResources.updateLocation,
			body: location
		});
	}

	/**
	 * Gets a list of Poppin-enabled businesses,
	 * based on a Yelp search
	 * @param {Object} searchParams 
	 */
	getLocationsByYelpList(searchParams) {
		try {
			const params = new YelpSearchEntities.YelpBusinessSearchParams(searchParams);
			return this.apiService.request({
				resource: HttpResources.getLocationsByYelpList,
				body: params
			});
		} catch (e) {
			console.error(e);
			return Promise.reject();
		}
	}
}
