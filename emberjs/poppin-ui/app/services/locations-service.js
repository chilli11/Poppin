import Service, { inject as injectService } from '@ember/service';
import HttpResources from '../utils/http-resources';

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
		return this.apiService.request({
			resource: HttpResources.getLocationsByYelpList,
			body: searchParams
		});
	}
}
