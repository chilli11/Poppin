import Service, { inject as injectService } from '@ember/service';
import HttpResources from '../utils/http-resources';
import { Promise } from 'rsvp';

export default class LocationsService extends Service {
	@injectService apiService;

	createNewVendor(location) {
		return this.apiService.request({
			resource: HttpResources.createNewLocation,
			body: location
		});
	}

	updateVendor(location) {
		return this.apiService.request({
			resource: HttpResources.updateLocation,
			body: location
		});
	}

	getVendorById(locId) {
		return this.apiService.request({
			resource: HttpResources.getLocation,
			body: { locId }
		});
	}

	/**
	 * Gets a list of Vendors,
	 * based on a list of Ids
	 * Usually will be SubVendor list
	 * @param {Object} searchParams 
	 */
	getVendorsByList(list) {
		try {
			return this.apiService.request({
				resource: HttpResources.getVendorsByList,
				body: list
			});
		} catch (e) {
			console.error(e);
			return Promise.reject();
		}
	}

	/**
	 * Gets a list of Vendors,
	 * based on a text search
	 * @param {Object} searchParams 
	 */
	getVendorsBySearch(searchTerm) {
		try {
			return this.apiService.request({
				resource: HttpResources.getVendorsBySearch,
				body: searchTerm
			});
		} catch (e) {
			console.error(e);
			return Promise.reject();
		}
	}
}
