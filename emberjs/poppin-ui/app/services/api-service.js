import Service, { inject } from '@ember/service';
import Evented from '@ember/object/evented';
import { tracked } from '@glimmer/tracking';
import { fetch } from 'fetch';
import { Promise } from 'rsvp';
import config from 'poppin-ui/config/environment';
import _ from 'lodash';

const GET = 'GET';
const POST = 'POST';

/**
 * @module Services/ApiService
 * @param {Object} httpResource
 * @param {HTTPRequest} fetchRequest
 * @example
 * getLocations = {
 *    url: 'api/locations/:locId',
 *   	method: POST,
 *    params: ['locId']
 *  };
 *  getYelpMatch = {
 *    url: 'api/yelp/match/:locId',
 *   	method: POST,
 *    params: ['locId'],
 *    keepData: ['locId']
 *  };
 *
 * this.request.getLocations({locId: 11}); // { url: 'api/locations/:locId', body: {}}
 * this.request.getYelpMatch({locId: 11}); // { url: 'api/yelp/match/:locId', body: { locId: 22}}
 */
export const paramsToSegments = (httpResource, fetchRequest) => {
	const options = _.merge(fetchRequest, { url: httpResource.url });
	const { body = {} } = options;
	(httpResource.params || []).forEach((param) => {
		const { url } = options;
		const inBody = param in body;
		let partOfURL = null;

		if (inBody) partOfURL = body[param];
		else
			console.error(`Parameter required for ${url}; check body of request for "${param}" key`);

		options.url = url.replace(`:${param}`, partOfURL);
		if ((httpResource.keepData || []).indexOf(param) === -1)
			options.body = _.omit(options.body, param);
	});
	if (options.body && !Object.keys(options.body).length) {
		options.body = null;
	}
	return options;
};

/**
 * @module Services/ApiService
 * @prop {Object} resources transformed `HttpResources`
 */
export default class ApiService extends Service.extend(Evented) {
	@inject router;
	@tracked jwt = sessionStorage.getItem('poppin_jwt');

	/**
	 * @param {String} url
	 */
	getJSON(url) {
		return fetch(config.rootURL + url).then((response) => {
			const isJson = response._bodyBlob && response._bodyBlob.type === 'application/json';
			return isJson ? response.json() : response.text();
		}).catch((response) => {
			const isJson = response._bodyBlob && response._bodyBlob.type === 'application/json';
			return isJson ? response.json() : response.text();
		});
	}


	/**
	 * Standard API request, used by all API methods that reach
	 * '/api/' URLs
	 * @instance
	 * @param {Object} options
	 * @param {Object} options.resource
	 * @param {String} options.resource.url
	 * @param {String} options.resource.method
	 * @param {String[]} [options.resource.params] uri parameter keys
	 * @param {Object} [options.params] key value pairs matching `options.resource.params`
	 * @param {Boolean} [options.cache=false]
	 * @param {Object|String} [options.body] data to post or send as query string
	 * @param {String} [options.dataType] def: 'json'
	 * @param {String} [options.contentType] def: 'application/json; charset=utf-8'
	 * @param {Object} [options.headers]
	 * @return {ApiResponse}
	 */
	request(options) {
		if (this.jwt) {
			options.headers = _.merge({
				Authorization: 'Bearer ' + this.jwt
			}, options.headers);
		}

		let fetchRequest = {
			url: '',
			body: options.body,
			params: options.params,
			cache: false,
			headers: _.merge({
				'Content-Type': options.contentType || 'application/json',
				Accept: 'application/json, text/*, */*',
			}, options.headers),
			withCredentials: true,
			credentials: 'include',
			mode: 'cors',
			method: options.resource.method || POST,
		};

		fetchRequest = paramsToSegments(options.resource, fetchRequest);
		const apiURL = config.apiURL.indexOf('http') == 0 ? config.apiURL : window.location.origin + config.apiURL;
		fetchRequest.url = new URL(apiURL + fetchRequest.url);
		if (fetchRequest.method !== GET) {
			fetchRequest.body = JSON.stringify(fetchRequest.body);
		} else {
			Object.keys(fetchRequest.body || {})
				.forEach(k => fetchRequest.url.searchParams.append(k, fetchRequest.body[k]));
			fetchRequest = _.omit(fetchRequest, 'body');
		}
		
		return new Promise((resolve, reject) => {
			const fn = (response) => {
				const isJson = response._bodyBlob && response._bodyBlob.type === 'application/json';
				const error = response.status > 399;
				let output = typeof response.text == 'function'? response.text() : response;
				
				output = isJson ? JSON.parse(output._result) : output._result;
				if (error) return reject(output);
				return resolve(output);
			};
			fetch(fetchRequest.url, fetchRequest).then(fn).catch(fn)
		});
	}

	unauthRedirect() {
		this.router.transitionTo('account');
	}
}
