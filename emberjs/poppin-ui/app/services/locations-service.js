import Service, { inject as injectService } from '@ember/service';
import HttpResources from '../utils/http-resources';
import YelpSearchEntities from '../classes/yelp-search-entities';
import LocationSearchEntities from '../classes/location-search-entities';
import { tracked } from '@glimmer/tracking';
import { Promise } from 'rsvp';

export default class LocationsService extends Service {
	@injectService apiService;

	@tracked lastSearch;

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

	getLocationById(locId) {
		return this.apiService.request({
			resource: HttpResources.getLocation,
			body: { locId }
		});
	}

	incrementCrowd(locId) {
		return this.apiService.request({
			resource: HttpResources.incrementCrowd,
			body: { locId }
		});
	}

	decrementCrowd(locId) {
		return this.apiService.request({
			resource: HttpResources.decrementCrowd,
			body: { locId }
		});
	}

	checkin(locId) {
		return this.apiService.request({
			resource: HttpResources.checkin,
			body: { locId }
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
			}).then((response) => {
				this.lastSearch = response;
				return response;
			});
		} catch (e) {
			console.error(e);
			return Promise.reject();
		}
	}

	/**
	 * Gets a list of Poppin-enabled businesses
	 * based on a native search
	 * @param {Object} searchParams 
	 */
	getLocationsBySearch(searchParams) {
		try {
			const params = new LocationSearchEntities.LocationSearchRequest(searchParams);
			return this.apiService.request({
				resource: HttpResources.getLocationsBySearch,
				body: params
			}).then((response) => {
				this.lastSearch = response;
				return response;
			});
		} catch (e) {
			console.error(e);
			return Promise.reject();
		}
	}
}
